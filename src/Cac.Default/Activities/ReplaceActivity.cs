using Cac.Default.Commands;
using Cac.Exceptions;
using Cac.Extensibility;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Default.Activities
{
    [Name("replace")]
    public class ReplaceActivity : ICacActivity
    {
        public Task<IEnumerable<ICacCommand>> PlanAsync(IYamlObject entry, IYamlConverter yamlConverter, IExecutionContext context)
        {
            if (entry is not YamlMappingObject node) throw new UnexpectedTypeException("Yaml node should be a mapping", entry.Line, entry.Column);
            return Task.FromResult(Plan(node, context));
        }

        public IEnumerable<ICacCommand> Plan(YamlMappingObject node, IExecutionContext context)
        {
            var command = new ReplaceCommand
            {
                Sourcefile = node.MapProperty<string>("sourcefile"),
                Targetfile = node.MapProperty<string>("targetfile"),
                Prefix = node.MapProperty<string>("prefix") ?? string.Empty,
                Sufix = node.MapProperty<string>("sufix") ?? string.Empty,
                Variables = new Dictionary<string, string>()
            };

            var variables = node.Children["variables"] as YamlMappingObject;
            foreach (var v in variables.Children)
            {
                command.Variables.Add(v.Key, (v.Value as YamlScalarObject)?.Value);
            }

            yield return command;
        }
    }
}
