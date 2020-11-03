using Cac.Extensibility;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System.IO;
using System.Threading.Tasks;

namespace Cac.Default.Providers
{
    [Name("directory")]
    public class DirectoryProvider : ICacProvider
    {
        public Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            if (node is YamlMappingObject m)
            {
                var path = m.MapProperty<string>("path");
                var pattern = (string)null;
                if (m.Children.ContainsKey("pattern"))
                { 
                    pattern = m.MapProperty<string>("pattern");
                }

                var result = GetFiles(path, pattern);
                result.Line = m.Line;
                result.Column = m.Column;
                return Task.FromResult(result);
            }

            return Task.FromResult<IYamlObject>(default);
        }

        public IYamlObject GetFiles(string path, string pattern)
        {
            var result = new YamlSequenceObject();
            if (!Directory.Exists(path)) return result;

            var files = string.IsNullOrWhiteSpace(pattern) ? Directory.GetFiles(path) : Directory.GetFiles(path, pattern);
            foreach (var file in files)
            {
                result.Add(new YamlScalarObject(file));
            }

            return result;
        }
    }
}
