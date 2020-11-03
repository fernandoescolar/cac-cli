using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;
using Cac.Yaml.Mapping;

namespace Cac.Expressions.Strategies
{
    public class NotFunctionStrategy : IEvalStrategy
    {
        public bool CanEval(Token token)
        {
            return token.Is(ExpressionTokenType.NotFunction);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            var arg1 = context.EvalNextObject().Map<bool>();
            return new YamlScalarObject(!arg1);
        }
    }
}
