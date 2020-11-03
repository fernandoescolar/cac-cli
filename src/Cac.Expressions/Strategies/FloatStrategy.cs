using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;

namespace Cac.Expressions.Strategies
{
    public class FloatStrategy : IEvalStrategy
    {
        public bool CanEval(Token token)
        {
            return token.Is(ExpressionTokenType.Float);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            return new YamlScalarObject(token.Value, YamlScalarType.Float);
        }
    }
}
