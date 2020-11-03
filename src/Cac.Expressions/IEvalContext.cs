using Cac.Yaml;

namespace Cac.Expressions
{
    public interface IEvalContext : IExecutionContext
    { 
        TokenStack Stack { get; }

        IYamlObject EvalNextObject();
    }
}
