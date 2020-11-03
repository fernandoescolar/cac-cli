using Cac.Exceptions;
using Cac.Expressions;
using Cac.Yaml;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cac.Interpretation
{
    public static class EvaluationExtensions
    {
        private static readonly Regex _regex = new Regex("\\${{[^}}]*}}", RegexOptions.IgnoreCase);

        public static void EvaluateExpressions(this Evaluator evaluator, IYamlObject yaml)
        {
            try
            {
                if (yaml is YamlScalarObject s) EvaluateExpressions(evaluator, s);
                else if (yaml is YamlSequenceObject l) EvaluateExpressions(evaluator, l);
                else if (yaml is YamlMappingObject m) EvaluateExpressions(evaluator, m);
                return;
            }
            catch (Exception ex)
            {
                throw new ExpressionEvaluationException(yaml.Line, yaml.Column, ex);
            }
            
            throw new UnexpectedTypeException("Yaml object not supported", yaml.Line, yaml.Column);
        }

        private static void EvaluateExpressions(this Evaluator evaluator, YamlScalarObject yaml)
        {
            while (HasExpression(yaml, out var match))
            {
                var result = EvaluateExpressions(evaluator, match);
                if (result is YamlScalarObject s)
                {
                    if (yaml.Value.Equals(match.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yaml.Value = s.Value;
                        yaml.YamlScalarType = s.YamlScalarType;
                    }
                    else
                    {
                        yaml.Value = yaml.Value.Replace(match.Value, s.Value);
                        yaml.YamlScalarType = YamlScalarType.String;
                    }
                }
                else throw new UnexpectedTypeException($"`YamlScalarObject` can not be assigned from `{result.GetType().Name}` object", yaml.Line, yaml.Column);
            }
        }

        private static void EvaluateExpressions(this Evaluator evaluator, YamlSequenceObject yaml)
        {
            var arr = new IYamlObject[yaml.Count];
            yaml.CopyTo(arr, 0);
            for (var i = 0; i < arr.Length; i++)
            {
                var child = arr[i];
                if (child is YamlScalarObject s)
                {
                    if (!HasExpression(s, out var match)) continue;

                    if (!s.Value.Equals(match.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        EvaluateExpressions(evaluator, s);
                    }
                    else
                    {
                        yaml[i] = EvaluateExpressions(evaluator, match);
                    }
                }
                else
                {
                    EvaluateExpressions(evaluator, child);
                }
            }
        }

        private static void EvaluateExpressions(this Evaluator evaluator, YamlMappingObject yaml)
        {
            if (yaml.Value is YamlScalarObject s && HasExpression(s, out var matchValue))
            {
                if (!s.Value.Equals(matchValue.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    EvaluateExpressions(evaluator, s);
                }
                else
                {
                    yaml.Value = EvaluateExpressions(evaluator, matchValue);
                }
            }

            foreach (var key in yaml.Children.Keys.ToList())
            {
                var child = yaml.Children[key];
                if (child is YamlScalarObject c)
                {
                    if (!HasExpression(c, out var matchChild)) continue;

                    if (!c.Value.Equals(matchChild.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        EvaluateExpressions(evaluator, c);
                    }
                    else
                    {
                        yaml.Children[key] = EvaluateExpressions(evaluator, matchChild);
                    }
                }
                else
                {
                    EvaluateExpressions(evaluator, child);
                }
            }
        }

        private static IYamlObject EvaluateExpressions(Evaluator evaluator, Match match)
        {
            var value = match.Value;
            var expression = value.Substring(3, value.Length - 5);
            var result = evaluator.Eval(expression);

            return result;
        }

        private static bool HasExpression(YamlScalarObject yaml, out Match result)
        {
            result = _regex.Match(yaml.Value);
            return result.Success;
        }
    }
}
