using Cac.Exceptions;
using Cac.Expressions;
using Cac.Yaml;
using System;

namespace Cac.Interpretation.NodeProcessors
{
    public class VariablesNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "variables".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            var mapping = node as YamlMappingObject ?? throw new UnexpectedTypeException("Yaml node should be a mapping", node.Line, node.Column);
            foreach (var entry in mapping.Children)
            {
                context.ExpressionEvaluator.EvaluateExpressions(entry.Value);
                context.Variables[entry.Key] = entry.Value;
                context.VerboseWriteAdding(entry.Key, entry.Value);
            }
        }
    }
}
