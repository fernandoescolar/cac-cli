using Cac.Yaml;
using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public interface ICacProvider
    {
        Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context);
    }
}
