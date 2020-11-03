using Cac.Exceptions;
using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;

namespace Cac.Expressions.Strategies
{
    public class ParenthesisBlockStrategy : IEvalStrategy
    {
        public bool CanEval(Token token)
        {
            return token.Is(ExpressionTokenType.OpenParenthesis);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            var yaml = context.EvalNextObject();
            var closeBrace = context.Stack.NextToken();

            if (closeBrace.Is(ExpressionTokenType.CloseParenthesis)
                && yaml is YamlScalarObject)
            {
                return yaml;
            }

            throw new UnexpectedCharacterException($"Can not evaluate parentesis block", closeBrace.Line, closeBrace.Column, closeBrace.Value);
        }
    }
}
