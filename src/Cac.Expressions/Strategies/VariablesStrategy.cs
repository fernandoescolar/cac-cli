using Cac.Expressions.Parser;
using Cac.Yaml;
using System.Collections.ObjectModel;

namespace Cac.Expressions.Strategies
{
    public class VariablesStrategy : ObjectPropertyStrategy
    {
        public VariablesStrategy() : base(ExpressionTokenType.Variables)
        {
        }

        protected override ReadOnlyDictionary<string, IYamlObject> GetObject(IEvalContext context)
        {
            return context.Variables;
        }
    }
}
