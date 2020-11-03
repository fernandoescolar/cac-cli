using Cac.Expressions.Parser;

namespace Cac.Expressions.Strategies
{
    public class OrFunctionStrategy : ScalarComparerStrategy
    {
        public OrFunctionStrategy() : base(ExpressionTokenType.OrFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = a || b;
            return true;
        }
    }
}
