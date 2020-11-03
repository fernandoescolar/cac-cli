using Cac.Expressions.Parser;
using System;

namespace Cac.Expressions.Strategies
{
    public class NotEqualFunctionStrategy : EqualFunctionStrategy
    {
        public NotEqualFunctionStrategy() : base(ExpressionTokenType.NotEqualFunction)
        {
        }

        protected override bool TryCompareObjects(bool a, bool b, out bool result)
        {
            var r = base.TryCompareObjects(a, b, out result);
            result = !result;
            return r;
        }

        protected override bool TryCompareObjects(int a, int b, out bool result)
        {
            var r = base.TryCompareObjects(a, b, out result);
            result = !result;
            return r;
        }

        protected override bool TryCompareObjects(float a, float b, out bool result)
        {
            var r = base.TryCompareObjects(a, b, out result);
            result = !result;
            return r;
        }

        protected override bool TryCompareObjects(string a, string b, out bool result)
        {
            var r = base.TryCompareObjects(a, b, out result);
            result = !result;
            return r;
        }

        protected override bool TryCompareObjects(Version a, Version b, out bool result)
        {
            var r = base.TryCompareObjects(a, b, out result);
            result = !result;
            return r;
        }
    }
}
