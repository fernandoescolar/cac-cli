using Cac.Expressions.Parser;
using System;

namespace Cac.Expressions.Strategies
{
    public class GreaterOrEqualFunctionStrategy : ScalarComparerStrategy
    {
        public GreaterOrEqualFunctionStrategy() : base(ExpressionTokenType.GreaterOrEqualFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = (a ? 1 : 0) >= (b ? 1 : 0);
            return true;
        }

        protected override bool TryCompareObjects(int a, int b, out bool result)
        {
            result = a >= b;
            return true;
        }

        protected override bool TryCompareObjects(float a, float b, out bool result)
        {
            result = a >= b;
            return true;
        }

        protected override bool TryCompareObjects(Version a, Version b, out bool result)
        {
            result = a >= b;
            return true;
        }
    }
}
