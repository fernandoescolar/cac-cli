using Cac.Yaml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public abstract class CacActivity<T> : ICacActivity 
        where T : class, new()
    {
        public Task<IEnumerable<ICacCommand>> PlanAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            var model = yamlConverter.To<T>(node);
            return OnPlanAsync(model, context);
        }

        protected abstract Task<IEnumerable<ICacCommand>> OnPlanAsync(T model, IExecutionContext context);
    }
}
