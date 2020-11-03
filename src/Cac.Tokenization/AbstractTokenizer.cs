using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cac.Tokenization
{
    public abstract class AbstractTokenizer
    {
        private readonly IEnumerable<TokenDefinition> _tokenDefinitions;

        public AbstractTokenizer(IEnumerable<TokenDefinition> tokenDefinitions)
        {
            _tokenDefinitions = tokenDefinitions;
        }

        public bool IgnoreWhitespaces { get; set; } = true;
        
        public bool IgnoreLineBreaks { get; set; } = false;
        
        public bool AddSequenceTerminatorToken { get; set; } = false;

        public List<Token> Tokenize(string text)
        {
            var cursor = new Cursor();
            var tokens = new List<Token>();
            var remainingText = text;
            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    tokens.Add(CreateToken(match, cursor));
                    remainingText = match.RemainingText;
                    cursor.Column += match.Value.Length;
                }
                else
                {
                    if (IsLineBreak(remainingText))
                    {
                        if (!IgnoreLineBreaks)
                        {
                            tokens.Add(CreateToken(GetLineBreakTokenType(), remainingText[0].ToString(), cursor));
                        }

                        cursor.Line++;
                        cursor.Column = 0;

                        remainingText = remainingText.Substring(1);
                    }
                    else if (IsWhitespace(remainingText))
                    {
                        if (!IgnoreWhitespaces)
                        {
                            tokens.Add(CreateToken(GetWhitespaceTokenType(), remainingText[0].ToString(), cursor));
                        }

                        cursor.Column++;
                        remainingText = remainingText.Substring(1);
                    }
                    else
                    {
                        var invalidTokenMatch = CreateUnknownTokenMatch(remainingText);
                        tokens.Add(CreateToken(invalidTokenMatch.TokenType, invalidTokenMatch.Value, cursor));
                        remainingText = invalidTokenMatch.RemainingText;
                        cursor.Column += invalidTokenMatch.Value.Length;
                    }
                }
            }

            if (AddSequenceTerminatorToken)
            {
                tokens.Add(CreateToken(GetSequenceTerminatorTokenType(), string.Empty, cursor));
            }

            return tokens;
        }

        protected virtual string GetSequenceTerminatorTokenType()
        {
            return "SequenceTerminator";
        }

        protected virtual string GetWhitespaceTokenType()
        {
            return "Whitespace";
        }

        protected virtual string GetLineBreakTokenType()
        {
            return "LineBreak";
        }

        protected virtual string GetStringTokenType()
        {
            return "String";
        }

        protected virtual string GetUnknownTokenType()
        {
            return "Unknown";
        }

        protected virtual string GetUnknownTokenDelimiters()
        {
            return " .:,;{}[]()¿?¡!%&$=·\"`´'/*-+@#^<>\n\r";
        }

        protected virtual bool IsWhitespace(string text)
        {
            return Regex.IsMatch(text, "^\\s+");
        }

        protected virtual bool IsLineBreak(string text)
        {
            return Regex.IsMatch(text, "^\\n+");
        }

        protected virtual TokenMatch CreateUnknownTokenMatch(string text)
        {
            var index = 0;
            while (index < text.Length)
            {
                var currentText = text[index].ToString();
                if (IsWhitespace(currentText) || IsLineBreak(currentText)) break;

                var delimiter = GetUnknownTokenDelimiters().Contains(currentText);
                if (delimiter) break;

                //var currentMatch = FindMatch(text.Substring(index));
                //if (currentMatch.IsMatch) break;

                index++;
            }

            var value = text.Substring(0, index);
            return new TokenMatch()
            {
                IsMatch = true,
                RemainingText = text.Substring(value.Length),
                TokenType = GetUnknownTokenType(),
                Value = value
            };
        }

        protected virtual Token CreateToken(string tokenType, string value, Cursor cursor)
        {
            if (tokenType.Equals(GetStringTokenType()))
            {
                return new StringToken(tokenType, value, cursor.Line, cursor.Column);
            }

            return new Token(tokenType, value, cursor.Line, cursor.Column);
        }

        private Token CreateToken(TokenMatch match, Cursor cursor)
        {
            return CreateToken(match.TokenType, match.Value, cursor);
        }

        private TokenMatch FindMatch(string text)
        {
            foreach (var tokenDefinition in _tokenDefinitions)
            {
                var match = tokenDefinition.Match(text);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };
        }
    }
}
