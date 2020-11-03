using Cac.Exceptions;
using Cac.Tokenization;
using System;
using System.Linq;
using System.Text;

namespace Cac.Yaml.Parser
{
    public class YamlParser
    {
        private static readonly YamlTokenType[] EndYamlTokenTypes = new[] { YamlTokenType.Comma, YamlTokenType.CloseBrace, YamlTokenType.CloseBracket };
        private readonly YamlTokenStream _stream;

        public YamlParser()
        {
            _stream = new YamlTokenStream();
        }

        public IYamlObject Parse(string content)
        {
            _stream.Initialize(content);

            var result = ParseRootObject();
            if (!_stream.HasEnded && result is not YamlMappingObject)
            {
                ThrowUnexpectedTokenExpection(_stream.GetNextNonSpaceToken());
            }

            return result;
        }

        private IYamlObject ParseRootObject()
        {
            var yamlObject = ParseYamlObject();

            if (yamlObject is YamlKeyValueObject child)
            {
                var result = new YamlMappingObject
                {
                    Line = yamlObject.Line,
                    Column = yamlObject.Column
                };

                result.ChildrenAdd(child.Key, child.Value);
                while (!_stream.HasEnded && _stream.NextLine())
                {
                    child = ParseKeyValueObject();
                    result.ChildrenAdd(child.Key, child.Value);
                }

                return result;
            }

            return yamlObject;
        }

        private IYamlObject ParseYamlObject()
        {
            var firstToken = _stream.GetNextNonSpaceTokenNoStep();
            var secondToken = _stream.GetNextNonSpaceTokenNoStep(1);

            if (firstToken.IsIn(YamlTokenType.StringValue, YamlTokenType.Unknown) && secondToken.Is(YamlTokenType.Colon))
            {
                var line = _stream.GetCurrentLine();
                var result = ParseKeyValueObject();
                if (line.First().Is(YamlTokenType.Dash))
                {
                    return ConvertToSequenceItem(result);
                }

                return result;
            }
            else if (firstToken.Is(YamlTokenType.Dash))
            {
                return ParseYamlSequenceObject();
            }
            else
            {
                return ParseYamlObjectValue();
            }
        }

        private YamlKeyValueObject ParseKeyValueObject()
        {
            (var key, var value) = ParseKeyValue();
            var line = _stream.GetCurrentLine();

            value.Line = Math.Max(value.Line, line.First().Line);
            value.Column = Math.Max(value.Column, line.First().Column);

            if (_stream.IsNextLineChildrenObject(line))
            {
                var obj = ParseYamlMappingObject(value);
                return new YamlKeyValueObject(key, obj) { Line = line.First().Line, Column = line.First().Column };
            }
            else if (_stream.IsNextLineChildrenSequence(line) 
                     && !line.First().Is(YamlTokenType.Dash)) // is an object with a sequence
            {
                _stream.NextLine();
                var obj = ParseYamlSequenceObject();
                if (!value.IsEmpty)
                {
                    obj.Value = value;
                }

                return new YamlKeyValueObject(key, obj) { Line = line.First().Line, Column = line.First().Column };
            }

            return new YamlKeyValueObject(key, value) { Line = line.First().Line, Column = line.First().Column };
        }

        private IYamlObject ParseYamlMappingObject(IYamlObject value)
        {
            var line = _stream.GetCurrentLine();
            var result = new YamlMappingObject { Value = value, Line = value.Line, Column = value.Column };
            var isFirst = true;
            while (_stream.IsNextLineChildren(line) && _stream.NextLine())
            {
                var nextToken = _stream.GetNextNonSpaceTokenNoStep();
                if (nextToken.Is(YamlTokenType.Dash))
                {
                    _stream.PreviousLine();
                    if (line.First().Is(YamlTokenType.Dash)) break;
                    ThrowUnexpectedTokenExpection(nextToken);
                }

                var child = ParseYamlObject();
                if (isFirst && child is not YamlKeyValueObject)
                {
                    if (!value.IsEmpty) ThrowUnexpectedLineExpection(line);
                    if (_stream.IsNextLineChildren(line))
                    {
                        _stream.NextLine();
                        ThrowUnexpectedLineExpection(_stream.GetCurrentLine());
                    }

                    return child;
                }

                isFirst = false;
                if (child is YamlKeyValueObject c)
                {
                    result.ChildrenAdd(c.Key, c.Value);
                }
                else
                {
                    ThrowUnexpectedTokenExpection(nextToken);
                }
            }

            return result;
        }

        private (string key, IYamlObject value) ParseKeyValue()
        {
            var token = _stream.GetNextNonSpaceToken();
            var colon = _stream.GetNextNonSpaceToken();

            if (!token.IsIn(YamlTokenType.StringValue, YamlTokenType.Unknown)) ThrowUnexpectedTokenExpection(token);
            if (!colon.Is(YamlTokenType.Colon)) ThrowUnexpectedTokenExpection(colon);

            return (token.Value, ParseYamlObjectValue());
        }

        private IYamlObject ConvertToSequenceItem(YamlKeyValueObject keyValue)
        {
            var result = new YamlMappingObject { Line = keyValue.Value.Line, Column = keyValue.Value.Column };
            if (keyValue.Value is YamlScalarObject)
            {
                result.ChildrenAdd(keyValue.Key, keyValue.Value);
            }
            else if (keyValue.Value is YamlMappingObject m)
            {
                result.ChildrenAdd(keyValue.Key, m.Value);
                m.Children.ToList().ForEach(x => result.ChildrenAdd(x.Key, x.Value));
            }
            else if (keyValue.Value is YamlSequenceObject s && s.Count == 1 && s.First() is YamlMappingObject sm)
            {
                foreach(var child in sm.Children)
                    result.ChildrenAdd(child.Key, child.Value);
            }
            else
            {
                ThrowUnexpectedLineExpection(_stream.GetCurrentLine());
            }

            return result;
        }

        private YamlSequenceObject ParseYamlSequenceObject()
        {
            var line = _stream.GetCurrentLine();
            var result = new YamlSequenceObject {  Line = line.First().Line, Column = line.First().Column };
            do 
            {
                var dash = _stream.GetNextNonSpaceToken();
                if (!dash.Is(YamlTokenType.Dash)) ThrowUnexpectedTokenExpection(dash);

                var child = ParseYamlObject();
                if (child is YamlScalarObject || child is YamlMappingObject)
                {
                    result.Add(child);
                }
                else
                {
                    ThrowUnexpectedLineExpection(_stream.GetCurrentLine());
                }
            } while (_stream.IsNextLineChildrenSequence(line) && _stream.NextLine()) ;

            return result;
        }

        private IYamlObject ParseYamlObjectValue()
        {
            var firstToken = _stream.GetNextNonSpaceTokenNoStep();
            if (firstToken.Is(YamlTokenType.StringValue))
            {
                return ParseStringYamlScalarObject();
            }
            else if (firstToken.Is(YamlTokenType.OpenBrace))
            {
                return ParseBraceYamlMappingObject();
            }
            else if (firstToken.Is(YamlTokenType.OpenBracket))
            {
                return ParseBracketYamlSequenceObject();
            }
            else 
            {
                return ParseYamlScalarObject();
            }
        }

        private IYamlObject ParseStringYamlScalarObject()
        {
            var token = _stream.GetNextNonSpaceToken();
            var nullToken = _stream.GetNextNonSpaceTokenNoStep();

            if (nullToken != null && !EndYamlTokenTypes.Contains(nullToken.GetYamlTokenType())) ThrowUnexpectedTokenExpection(nullToken);

            return new YamlScalarObject(token.Value, YamlScalarType.String) { Line = token.Line, Column = token.Column };
        }

        private IYamlObject ParseBracketYamlSequenceObject()
        {
            _stream.IgnoreNextNonSpaceToken(); // open bracket
            var nextToken = _stream.GetNextNonSpaceTokenNoStep();
            var result = new YamlSequenceObject { Line = nextToken.Line, Column = nextToken.Column };
            while (nextToken != null)
            {
                result.Add(ParseYamlObjectValue());
                nextToken = _stream.GetNextNonSpaceToken();

                if (nextToken.Is(YamlTokenType.CloseBracket)) break;
                if (nextToken.Is(YamlTokenType.Comma)) continue;
                if (nextToken == null) ThrowUnexpectedEndOfLine(_stream.GetCurrentLine());
                ThrowUnexpectedTokenExpection(nextToken);
            }

            if (!nextToken.Is(YamlTokenType.CloseBracket)) ThrowUnexpectedTokenExpection(nextToken);
            return result;
        }

        private IYamlObject ParseBraceYamlMappingObject()
        {
            _stream.IgnoreNextNonSpaceToken(); // open brace
            var nextToken = _stream.GetNextNonSpaceTokenNoStep();
            var result = new YamlMappingObject { Line = nextToken.Line, Column = nextToken.Column };
            while (nextToken !=null)
            {
                var (key, value) = ParseKeyValue();
                result.ChildrenAdd(key, value);
                
                nextToken = _stream.GetNextNonSpaceToken();
                if (nextToken.Is(YamlTokenType.CloseBrace)) break;
                if (nextToken.Is(YamlTokenType.Comma)) continue;
                if (nextToken == null) ThrowUnexpectedEndOfLine(_stream.GetCurrentLine());
                ThrowUnexpectedTokenExpection(nextToken);
            }

            if (!nextToken.Is(YamlTokenType.CloseBrace)) ThrowUnexpectedTokenExpection(nextToken);
            return result;
        }

        private IYamlObject ParseYamlScalarObject()
        {
            var firstToken = _stream.GetNextNonSpaceToken();
            var secondToken = _stream.GetNextNonSpaceTokenNoStep();
            if (secondToken == null || EndYamlTokenTypes.Contains(secondToken.GetYamlTokenType()))
            {
                return ParseYamlScalarObject(firstToken);
            }

            return ParseYamlScalarObjectUntilEndOfLine(firstToken);
        }

        private IYamlObject ParseYamlScalarObject(Token token)
        {
            if (token == null) return new YamlScalarObject(string.Empty, YamlScalarType.Null);
            if (token.Is(YamlTokenType.Boolean)) return new YamlScalarObject(token.Value, YamlScalarType.Boolean) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Integer)) return new YamlScalarObject(token.Value, YamlScalarType.Int) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Float)) return new YamlScalarObject(token.Value, YamlScalarType.Float) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Version)) return new YamlScalarObject(token.Value, YamlScalarType.Version) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.DateTime)) return new YamlScalarObject(token.Value, YamlScalarType.DateTime) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Null)) return new YamlScalarObject(token.Value, YamlScalarType.Null) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.StringValue)) return new YamlScalarObject(token.Value, YamlScalarType.String) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Unknown)) return new YamlScalarObject(token.Value, YamlScalarType.String) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.Expression)) return new YamlScalarObject(token.Value, YamlScalarType.String) { Line = token.Line, Column = token.Column };
            if (token.Is(YamlTokenType.GreaterThan)) return ParseStringYamlScalarObject(token);
            if (token.Is(YamlTokenType.Pipe)) return ParseStringYamlScalarObject(token);

            ThrowUnexpectedTokenExpection(token);
            return default;
        }

        private IYamlObject ParseYamlScalarObjectUntilEndOfLine(Token firstToken)
        {
            if (EndYamlTokenTypes.Contains(firstToken.GetYamlTokenType())) ThrowUnexpectedTokenExpection(firstToken);
            if (firstToken.Is(YamlTokenType.Dash)) ThrowUnexpectedTokenExpection(firstToken);
            if (firstToken.Is(YamlTokenType.Colon)) ThrowUnexpectedTokenExpection(firstToken);

            var nextToken = _stream.GetNextToken();
            if (firstToken.Is(YamlTokenType.StringValue)) ThrowUnexpectedTokenExpection(nextToken);
            if (firstToken.Is(YamlTokenType.GreaterThan)) ThrowUnexpectedTokenExpection(nextToken);
            if (firstToken.Is(YamlTokenType.Pipe)) ThrowUnexpectedTokenExpection(nextToken);

            var line = nextToken.Line;
            var column = nextToken.Column;
            var sb = new StringBuilder();
            sb.Append(firstToken.Value);
            
            do {
                if (nextToken.Is(YamlTokenType.StringValue))
                {
                    sb.Append("\"");
                    sb.Append(nextToken.Value);
                    sb.Append("\"");
                }
                else
                {
                    sb.Append(nextToken.Value);
                }
                
                nextToken = _stream.GetNextTokenNoStep();
                if (nextToken == null /*&& EndYamlTokenTypes.Contains(nextToken.GetYamlTokenType())*/) break;
                _stream.IgnoreNextToken();
            } while (nextToken  != null);

            return new YamlScalarObject(sb.ToString(), YamlScalarType.String) { Line = line, Column = column };
        }

        private IYamlObject ParseStringYamlScalarObject(Token token)
        {
            var ignoreBreaks = token.Value.Contains(">");
            var ignoreLastBreak = token.Value.Contains("-");
            var line = _stream.GetCurrentLine();
            var sb = new StringBuilder();
            while (_stream.IsNextLineChildrenObject(line) && _stream.NextLine())
            {
                sb.Append(_stream.GetCurrentLine().Text);
                sb.Append(ignoreBreaks ? " " : "\n");
            }

            var str = sb.ToString();
            if (str.EndsWith(" "))
            {
                str = str.Substring(0, str.Length - 1);
            }

            if (str.EndsWith("\n") && ignoreLastBreak)
            {
                str = str.Substring(0, str.Length - 1);
            }

            if (!str.EndsWith("\n") && !ignoreLastBreak)
            {
                str += "\n";
            }

            return new YamlScalarObject(str, YamlScalarType.String) { Line = token.Line, Column = token.Column };
        }

        private void ThrowUnexpectedTokenExpection(Token token)
        {
            throw new UnexpectedCharacterException(token.Line, token.Column, token.Value);
        }

        private void ThrowUnexpectedLineExpection(LineOfTokens line)
        {
            throw new UnexpectedBeginOfLineException(line.First().Line, line.First().Value);
        }

        private void ThrowUnexpectedEndOfLine(LineOfTokens line)
        {
            throw new Exception($"Not expected end of line: {line.First().Line}");
        }
    }
}
