using Cac.Tokenization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cac.Yaml.Parser
{
    internal class LineOfTokens : IEnumerable<Token>
    {
        private readonly IEnumerable<Token> _tokens;
        private readonly Lazy<int> _spaces;
        private readonly Lazy<string> _text;
        private readonly Lazy<IEnumerable<Token>> _filteredTokens;

        public LineOfTokens(IEnumerable<Token> tokens)
        {
            _tokens = tokens;
            _spaces = new Lazy<int>(GetLineBeginingSpacesCount);
            _text = new Lazy<string>(GetLineTextWithoutBeginingSpaces);
            _filteredTokens = new Lazy<IEnumerable<Token>>(() => GetFilteredTokens().ToList());
        }

        public int Spaces => _spaces.Value;

        public string Text => _text.Value;

        public IEnumerator<Token> GetEnumerator()
        {
            return _filteredTokens.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filteredTokens.Value.GetEnumerator();
        }

        private IEnumerable<Token> GetFilteredTokens()
        {
            var ignore_spaces = true;
            foreach (var token in _tokens)
            {
                if (ignore_spaces && token.TokenType == nameof(YamlTokenType.Space)) continue;
                if (token.TokenType == nameof(YamlTokenType.Comment)) continue;
                ignore_spaces = false;
                yield return token;
            }
        }

        private int GetLineBeginingSpacesCount()
        {
            var sum = 0;
            foreach (var token in _tokens)
            {
                if (token.TokenType != nameof(YamlTokenType.Space))
                {
                    break;
                }

                var value = token.Value.Replace("\t", "    ");
                sum += value.Length;
            }

            return sum;
        }

        private string GetLineTextWithoutBeginingSpaces()
        {
            var sb = new StringBuilder();
            var ignore_spaces = true;
            foreach (var token in _tokens)
            {
                if (ignore_spaces && token.TokenType == nameof(YamlTokenType.Space)) continue;
                ignore_spaces = false;
                sb.Append(token.Value);
            }

            return sb.ToString();
        }
    }
}
