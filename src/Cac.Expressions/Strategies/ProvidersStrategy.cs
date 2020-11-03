using Cac.Expressions.Parser;
using Cac.Yaml;
using System.Collections.ObjectModel;

namespace Cac.Expressions.Strategies
{
    public class ProvidersStrategy : ObjectPropertyStrategy
    {
        public ProvidersStrategy() : base(ExpressionTokenType.Providers)
        {
        }

        protected override ReadOnlyDictionary<string, IYamlObject> GetObject(IEvalContext context)
        {
            return context.Providers;
        }
    }
}
