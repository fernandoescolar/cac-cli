using System.Text.RegularExpressions;

namespace Cac.Tokenization
{
    public class TokenDefinition
    {
        private readonly Regex _regex;
        private readonly string _returnsToken;
        private readonly bool _wholeWord;

        public TokenDefinition(string returnsToken, string regexPattern, bool wholeWord = false)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
            _wholeWord = wholeWord;
        }

        public TokenMatch Match(string inputString)
        {
            var match = _regex.Match(inputString);
            if (match.Success)
            {
                var value = match.Value;
                if (value.EndsWith("\r\n"))
                {
                    value = value.Substring(0, value.Length - 2);
                }
                else if (value.EndsWith("\n"))
                {
                    value = value.Substring(0, value.Length - 1);
                }

                var remainingText = string.Empty;
                if (value.Length < inputString.Length)
                {
                    remainingText = inputString.Substring(value.Length);
                }

                if (_wholeWord && HasNotWholeWordRemaingText(remainingText[0]))
                {
                    return new TokenMatch() { IsMatch = false };
                }

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = _returnsToken,
                    Value = value
                };
            }
            else
            {
                return new TokenMatch() { IsMatch = false };
            }
        }

        private bool HasNotWholeWordRemaingText(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }
    }
}
