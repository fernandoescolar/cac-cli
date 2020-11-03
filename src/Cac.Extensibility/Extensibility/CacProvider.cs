using Cac.Yaml;
using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public abstract class CacProvider<T> : ICacProvider
          where T : class, new()
    {
        public async Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            var result = await OnGetValueAsync(context);
            return yamlConverter.From(result);
        }

        protected abstract Task<T> OnGetValueAsync(IExecutionContext context);
    }

    public abstract class CacProvider<T, S> : ICacProvider
          where T : class, new()
    {
        public async Task<IYamlObject> GetValueAsync(IYamlObject node, IYamlConverter yamlConverter, IExecutionContext context)
        {
            var model = yamlConverter.To<T>(node);
            var result = await OnGetValueAsync(model, context);
            return yamlConverter.From(result);
        }

        protected abstract Task<S> OnGetValueAsync(T model, IExecutionContext context);
    }
}
