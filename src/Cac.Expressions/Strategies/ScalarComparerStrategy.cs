using Cac.Exceptions;
using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System;

namespace Cac.Expressions.Strategies
{
    public abstract class ScalarComparerStrategy : IEvalStrategy
    {
        private readonly ExpressionTokenType _type;

        protected ScalarComparerStrategy(ExpressionTokenType type)
        {
            _type = type;
        }

        public bool CanEval(Token token)
        {
            return token.Is(_type);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            var openBrace = context.Stack.NextToken();
            var yaml1 = context.EvalNextObject();
            var comma = context.Stack.NextToken();
            var yaml2 = context.EvalNextObject();
            var closeBrace = context.Stack.NextToken();

            if (openBrace.Is(ExpressionTokenType.OpenParenthesis) 
                && comma.Is(ExpressionTokenType.Comma)
                && closeBrace.Is(ExpressionTokenType.CloseParenthesis)
                && yaml1 is YamlScalarObject t1
                && yaml2 is YamlScalarObject t2
                && TryCompareYamlScalarObjects(t1, t2, out var result))
            {
                return new YamlScalarObject(result);
            }

            throw new UnexpectedTypeException($"Can not perform comparation {_type} between: '{yaml1}' and '{yaml2}'", yaml2.Line, yaml2.Column);
        }

        protected bool TryCompareYamlScalarObjects(YamlScalarObject a, YamlScalarObject b, out bool result)
        {
            if (a.YamlScalarType == b.YamlScalarType && a.YamlScalarType == YamlScalarType.Boolean)
                return TryCompareObjects(a.Map<bool>(), b.Map<bool>(), out result);
            else if (a.YamlScalarType == b.YamlScalarType && a.YamlScalarType == YamlScalarType.String)
                return TryCompareObjects(a.Map<string>(), b.Map<string>(), out result);
            else if (a.YamlScalarType == b.YamlScalarType && a.YamlScalarType == YamlScalarType.Version)
                return TryCompareObjects(a.Map<Version>(), b.Map<Version>(), out result);
            else if (a.YamlScalarType == b.YamlScalarType && a.YamlScalarType == YamlScalarType.Int)
                return TryCompareObjects(a.Map<int>(), b.Map<int>(), out result);
            else if (a.YamlScalarType == b.YamlScalarType && a.YamlScalarType == YamlScalarType.Float)
                return TryCompareObjects(a.Map<float>(), b.Map<float>(), out result);
            else if (a.YamlScalarType == YamlScalarType.Int && b.YamlScalarType == YamlScalarType.Float)
                return TryCompareObjects(a.Map<float>(), b.Map<float>(), out result);
            else if (a.YamlScalarType == YamlScalarType.Float && b.YamlScalarType == YamlScalarType.Int)
                return TryCompareObjects(a.Map<float>(), b.Map<float>(), out result);

            result = default;
            return default;
        }

        protected virtual bool TryCompareObjects(bool a, bool b, out bool result)
        {
            result = default;
            return default;
        }

        protected virtual bool TryCompareObjects(int a, int b, out bool result)
        {
            result = default;
            return default;
        }

        protected virtual bool TryCompareObjects(float a, float b, out bool result)
        {
            result = default;
            return default;
        }

        protected virtual bool TryCompareObjects(string a, string b, out bool result)
        {
            result = default;
            return default;
        }

        protected virtual bool TryCompareObjects(Version a, Version b, out bool result)
        {
            result = default;
            return default;
        }
    }
}
