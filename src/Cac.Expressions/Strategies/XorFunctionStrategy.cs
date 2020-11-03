using Cac.Expressions.Parser;

namespace Cac.Expressions.Strategies
{
    public class XorFunctionStrategy : ScalarComparerStrategy
    {
        public XorFunctionStrategy() : base(ExpressionTokenType.XorFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = a ^ b;
            return true;
        }
    }
}
