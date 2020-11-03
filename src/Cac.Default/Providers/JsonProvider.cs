using Cac.Extensibility;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cac.Default.Providers
{
    [Name("json")]
    public class JsonProvider : ICacProvider
    {
        public async Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            if (node is YamlMappingObject m)
            {
                var file = m.MapProperty<string>("file");
                ChecksFileExists(file);

                var result = await LoadFileAsync(file);
                result.Line = m.Line;
                result.Column = m.Column;

                return result;
            }

            return null;
        }

        private static void ChecksFileExists(string file)
        {
            if (File.Exists(file)) return;
            throw new FileNotFoundException("Json file not found", file);
        }

        private async Task<IYamlObject> LoadFileAsync(string file)
        {
            var content = await File.ReadAllTextAsync(file);
            var token = JToken.Parse(content);
            return ParseJToken(token);
        }

        private static IYamlObject ParseJToken(JToken token)
        {
            return token.Type switch
            {
                JTokenType.Object => ParseObject(token),
                JTokenType.Array => ParseArray(token),
                JTokenType.Boolean => new YamlScalarObject((bool)token),
                JTokenType.Date => new YamlScalarObject((DateTime)token),
                JTokenType.Float => new YamlScalarObject((float)token),
                JTokenType.Guid => new YamlScalarObject(((Guid)token).ToString()),
                JTokenType.Integer => new YamlScalarObject((int)token),
                JTokenType.None => new YamlScalarObject(),
                JTokenType.Null => new YamlScalarObject(),
                JTokenType.String => new YamlScalarObject((string)token),
                JTokenType.Undefined => new YamlScalarObject(),
                JTokenType.Uri => new YamlScalarObject(((Uri)token).ToString()),
                JTokenType.Bytes => null,
                JTokenType.Comment => null,
                JTokenType.Constructor => null,
                JTokenType.TimeSpan => null,
                _ => null
            };
        }

        private static IYamlObject ParseObject(JToken token)
        {
            var result = new YamlMappingObject();
            var jobject = (JObject)token;
            foreach (var o in jobject.Properties())
            {
                result.ChildrenAdd(o.Name, ParseJToken(o.Value));
            }

            return result;
        }

        private static IYamlObject ParseArray(JToken token)
        {
            var result = new YamlSequenceObject();
            foreach (var o in token)
            {
                result.Add(ParseJToken(o));
            }

            return result;
        }
    }
}
