using Cac.Tokenization;
using Cac.Yaml;

namespace Cac.Expressions
{
    public interface IEvalStrategy
    {
        bool CanEval(Token token);

        IYamlObject Eval(IEvalContext context, Token token);
    }
}
