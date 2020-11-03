using Cac.Expressions.Parser;

namespace Cac.Expressions.Strategies
{
    public class AndFunctionStrategy : ScalarComparerStrategy
    {
        public AndFunctionStrategy() : base(ExpressionTokenType.AndFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = a && b;
            return true;
        }
    }
}
