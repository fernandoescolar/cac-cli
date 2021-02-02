using Cac.Exceptions;
using Cac.Expressions;
using Cac.Extensibility;
using Cac.Options;
using Cac.Output;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using Cac.Yaml.Parser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Cac.Default.Providers
{
    [Name("yaml")]
    public class YamlProvider : ICacProvider
    {
        public async Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            if (node is YamlMappingObject m)
            {
                var file = m.MapProperty<string>("file");
                ChecksFileExists(file);

                var parameters = ParseParameters(m);
                var result = await LoadFileAsync(file);
                result.Line = m.Line;
                result.Column = m.Column;

                EvaluateExpressions(result, context, parameters);

                return result;
            }

            return null;
        }

        private static void ChecksFileExists(string file)
        {
            if (File.Exists(file)) return;
            throw new FileNotFoundException("Yaml file not found", file);
        }

        private static async Task<IYamlObject> LoadFileAsync(string file)
        {
            var content = await File.ReadAllTextAsync(file);
            var parser = new YamlParser();
            return parser.Parse(content);
        }

        private static Dictionary<string, IYamlObject> ParseParameters(YamlMappingObject entry)
        {
            var result = new Dictionary<string, IYamlObject>();
            if (entry.Children.ContainsKey("parameters"))
            {
                var mapping = entry.Children["parameters"] as YamlMappingObject ?? throw new UnexpectedTypeException("Yaml node should be a mapping", entry.Line, entry.Column);
                foreach (var item in mapping.Children)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        private static void EvaluateExpressions(IYamlObject entry, IExecutionContext context, Dictionary<string, IYamlObject> parameters)
        {
            var ctx = new ExecutionContext(context, parameters);
            var evaluator = new Evaluator(ctx);
            evaluator.EvaluateExpressions(entry);
        }

        private class ExecutionContext : IExecutionContext
        {
            private readonly IExecutionContext _context;
            private readonly Dictionary<string, IYamlObject> _parameters;

            public ExecutionContext(IExecutionContext context, Dictionary<string, IYamlObject> parameters)
            {
                _context = context;
                _parameters = parameters;
            }

            public ReadOnlyDictionary<string, IYamlObject> Parameters => new ReadOnlyDictionary<string, IYamlObject>(new Dictionary<string, IYamlObject>());

            public ReadOnlyDictionary<string, IYamlObject> Variables => new ReadOnlyDictionary<string, IYamlObject>(new Dictionary<string, IYamlObject>());

            public ReadOnlyDictionary<string, IYamlObject> Providers => new ReadOnlyDictionary<string, IYamlObject>(new Dictionary<string, IYamlObject>());

            public Dictionary<string, IYamlObject> Locals => _parameters;

            public List<ICacCommand> Commands => new List<ICacCommand>();

            public ICacOptions Options => _context.Options;

            public IOutput Out => _context.Out;
        }
    }
}
