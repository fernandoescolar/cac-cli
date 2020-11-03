using Cac.Exceptions;
using Cac.Yaml;
using System;

namespace Cac.Interpretation.NodeProcessors
{
    public class ParametersNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "parameters".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            var mapping = node as YamlMappingObject ?? throw new UnexpectedTypeException("Yaml node should be a mapping", node.Line, node.Column);
            foreach (var entry in mapping.Children)
            {
                var parameterName = entry.Key;
                context.Parameters[parameterName] = entry.Value;
                context.VerboseWriteAdding(parameterName, entry.Value);
            }
        }
    }
}
