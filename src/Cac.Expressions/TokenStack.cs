using Cac.Tokenization;
using System.Collections.Generic;
using System.Linq;

namespace Cac.Expressions
{
    public class TokenStack
    {
        private readonly List<Token> _tokens;
        private int _position;

        public TokenStack(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _position = 0;
        }

        public bool HasEnded => _position >= _tokens.Count - 1;

        public Token NextToken()
        {
            return _position < _tokens.Count ? _tokens[_position++] : default;
        }

        public void StepBack()
        {
            if (_position > 0) _position--;
        }
    }
}
