using Cac.Yaml;

namespace Cac.Interpretation.NodeProcessors
{
    public interface INodeProcessor
    {
        bool CanProcess(string key);

        void Process(IYamlObject node, IExecutionContext context);
    }
}
