using Cac.Expressions.Parser;
using System;

namespace Cac.Expressions.Strategies
{
    public class EqualFunctionStrategy : ScalarComparerStrategy
    {
        protected EqualFunctionStrategy(ExpressionTokenType type) : base(type)
        { 
        }

        public EqualFunctionStrategy() : this(ExpressionTokenType.EqualFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = a == b;
            return true;
        }

        protected override bool TryCompareObjects(int a, int b, out bool result)
        {
            result = a == b;
            return true;
        }

        protected override bool TryCompareObjects(float a, float b, out bool result)
        {
            result = a == b;
            return true;
        }

        protected override bool TryCompareObjects(string a, string b, out bool result)
        {
            result = (a != null && a.Equals(b, StringComparison.InvariantCulture)) || (a == null && b == null);
            return true;
        }

        protected override bool TryCompareObjects(Version a, Version b, out bool result)
        {
            result = a == b;
            return true;
        }
    }
}
