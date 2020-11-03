using Cac.Yaml;
using System;

namespace Cac.Interpretation.NodeProcessors
{
    public class ParametersFromOptionsNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "parameters".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            foreach (var item in context.Options.Parameters)
            {
                var parts = item.Split(new[] { "=" }, StringSplitOptions.None);
                var key = parts[0];
                var value = new YamlScalarObject(parts[1]);
                context.Parameters[key] = value;
                context.VerboseWriteAdding(key, value);
            }
        }
    }
}
