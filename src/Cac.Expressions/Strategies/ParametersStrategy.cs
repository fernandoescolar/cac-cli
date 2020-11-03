using Cac.Expressions.Parser;
using Cac.Yaml;
using System.Collections.ObjectModel;

namespace Cac.Expressions.Strategies
{
    public class ParametersStrategy : ObjectPropertyStrategy
    {
        public ParametersStrategy() : base(ExpressionTokenType.Parameters)
        {
        }

        protected override ReadOnlyDictionary<string, IYamlObject> GetObject(IEvalContext context)
        {
            return context.Parameters;
        }
    }
}
