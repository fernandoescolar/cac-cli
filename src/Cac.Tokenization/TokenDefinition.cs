using System.Text.RegularExpressions;

namespace Cac.Tokenization
{
    public class TokenDefinition
    {
        private readonly Regex _regex;
        private readonly string _returnsToken;

        public TokenDefinition(string returnsToken, string regexPattern)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
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
    }
}
