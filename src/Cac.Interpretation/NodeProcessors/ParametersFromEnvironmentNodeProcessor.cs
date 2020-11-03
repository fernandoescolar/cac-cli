using Cac.Yaml;
using System;
using System.Linq;

namespace Cac.Interpretation.NodeProcessors
{
    public class ParametersFromEnvironmentNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "parameters".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            foreach (var key in context.Parameters.Keys.ToList())
            {
                var environmentValue = Environment.GetEnvironmentVariable(key);
                if (environmentValue != null)
                {
                    context.Parameters[key] = new YamlScalarObject(environmentValue);
                    context.VerboseWriteAdding(key, context.Parameters[key]);
                }
            }
        }
    }
}
