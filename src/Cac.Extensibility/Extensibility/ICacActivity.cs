using Cac.Yaml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public interface ICacActivity
    {
        Task<IEnumerable<ICacCommand>> PlanAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context);
    }
}
