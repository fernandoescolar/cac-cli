using Cac.Tokenization;
using System.Collections.Generic;
using System.Linq;

namespace Cac.Yaml.Parser
{
    internal class YamlTokenStream
    {
        private readonly YamlTokenizer _tokenizer;
        private readonly Cursor _cursor;
        private List<LineOfTokens> _lines;

        public YamlTokenStream()
        {
            _tokenizer = new YamlTokenizer();
            _cursor = new Cursor();
        }

        public bool HasEnded => _cursor.Line >= _lines.Count - 1;

        public void Initialize(string content)
        {
            var tokens = _tokenizer.Tokenize(content);
            var lines = SplitInLines(tokens);
            _lines = lines.ToList();
            _cursor.Reset();
        }

        public Token GetNextToken()
        {
            return _lines[_cursor.Line].Skip(_cursor.Column++).FirstOrDefault();
        }

        public void IgnoreNextToken()
        {
            GetNextToken();
        }

        public Token GetNextNonSpaceToken()
        {
            Token token;
            while ((token = GetNextToken()) != null)
            {
                if (!token.Is(YamlTokenType.Space)) return token;
            }

            return default;
        }

        public void IgnoreNextNonSpaceToken()
        {
            GetNextNonSpaceToken();
        }

        public Token GetNextTokenNoStep(int position = 0)
        {
            return _lines[_cursor.Line].Skip(_cursor.Column + position).FirstOrDefault();
        }

        public Token GetNextNonSpaceTokenNoStep(int position = 0)
        {
            Token token;
            var counter = 0;
            var innerCounter = 0;
            while ((token = GetNextTokenNoStep(innerCounter)) != null)
            {
                innerCounter++;
                if (token.Is(YamlTokenType.Space)) continue;
                if (counter++ == position) return token;
            }

            return default;
        }

        public bool NextLine()
        {
            if (_cursor.Line + 1 < _lines.Count)
            {
                _cursor.Line++;
                _cursor.Column = 0;

                return true;
            }

            return false;
        }

        public bool PreviousLine()
        {
            if (_cursor.Line - 1 >= 0)
            {
                _cursor.Line--;
                _cursor.Column = 0;

                return true;
            }

            return false;
        }

        public bool IsNextLineChildren(LineOfTokens line)
        {
            return IsNextLineChildrenObject(line) || IsNextLineChildrenSequence(line);
        }

        public bool IsNextLineChildrenObject(LineOfTokens line)
        {
            return _cursor.Line + 1 < _lines.Count
                   && _lines[_cursor.Line + 1].Spaces > line.Spaces;
        }

        public bool IsNextLineChildrenSequence(LineOfTokens line)
        {
            return _cursor.Line + 1 < _lines.Count
                   && _lines[_cursor.Line + 1].Spaces == line.Spaces
                   && _lines[_cursor.Line + 1].FirstOrDefault().Is(YamlTokenType.Dash);
        }

        public LineOfTokens GetCurrentLine()
        {
            return _lines[_cursor.Line];
        }

        private IEnumerable<LineOfTokens> SplitInLines(IEnumerable<Token> tokens)
        {
            var buffer = new List<Token>();
            foreach (var token in tokens)
            {
                if (token.TokenType == nameof(YamlTokenType.LineBreak))
                {
                    var line = new LineOfTokens(buffer);
                    if (line.Any()) yield return line;
                    buffer = new List<Token>();
                    continue;
                }

                buffer.Add(token);
            }

            var lastLine = new LineOfTokens(buffer);
            if (lastLine.Any()) yield return lastLine;
        }
    }
}
