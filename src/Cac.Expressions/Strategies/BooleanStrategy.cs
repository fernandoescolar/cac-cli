using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;

namespace Cac.Expressions.Strategies
{
    public class BooleanStrategy : IEvalStrategy
    {
        public bool CanEval(Token token)
        {
            return token.Is(ExpressionTokenType.Boolean);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            return new YamlScalarObject(token.Value, YamlScalarType.Boolean);
        }
    }
}
