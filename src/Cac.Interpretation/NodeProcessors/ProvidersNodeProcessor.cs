using Cac.Exceptions;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cac.Interpretation.NodeProcessors
{
    public class ProvidersNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "providers".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            var sequence = node as YamlSequenceObject ?? throw new UnexpectedTypeException("Yaml node should be a sequence", node.Line, node.Column);
            var tasks = new List<Task>();
            foreach (var entry in sequence)
            {
                tasks.Add(ProcessEntryAsync(entry, context));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private async Task ProcessEntryAsync(IYamlObject entry, IExecutionContext context)
        {
            context.ExpressionEvaluator.EvaluateExpressions(entry);
            if (entry is YamlMappingObject m)
            {
                var providerName = m.Children.First().Key;
                var propertyName = m.Children.First().Value.Map<string>();
                var result = await ExecuteProviderAsync(context, entry, providerName);

                context.Providers[propertyName] = result;
                context.VerboseWriteAdding(propertyName, result);
                return;
            }

            throw new UnexpectedTypeException("Yaml node should be a mapping", entry.Line, entry.Column);
        }

        private static Task<IYamlObject> ExecuteProviderAsync(IExecutionContext context, IYamlObject entry, string providerName)
        {
            var key = context.AvailableProviders.Keys.FirstOrDefault(x => x.Equals(providerName, StringComparison.InvariantCultureIgnoreCase));
            if (key == null)
            {
                context.Out.Warning($"Provider `{providerName}` not found");
                return default;
            }

            context.Out.Verbose.Write("executing provider `");
            context.Out.Verbose.Write(providerName, ConsoleColor.Green);
            context.Out.Verbose.WriteLine("`");
            return context.AvailableProviders[key].GetValueAsync(entry, context.YamlConverter, context);
        }
    }
}
