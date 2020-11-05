using Cac.Tokenization;
using System.Collections.Generic;

namespace Cac.Expressions.Parser
{
    public class ExpressionsTokenizer : AbstractTokenizer
    {
        public ExpressionsTokenizer() :
            base(new List<TokenDefinition> {
                new TokenDefinition(nameof(ExpressionTokenType.Null), "^null", true),
                new TokenDefinition(nameof(ExpressionTokenType.Boolean), "^true|false", true),
                new TokenDefinition(nameof(ExpressionTokenType.Integer), "^\\d+"),
                new TokenDefinition(nameof(ExpressionTokenType.Integer), "^0x[a-fA-F0-9]+"),
                new TokenDefinition(nameof(ExpressionTokenType.Float), "^\\d+\\.\\d+"),
                new TokenDefinition(nameof(ExpressionTokenType.StringValue), "^'[^']*'"),
                new TokenDefinition(nameof(ExpressionTokenType.StringValue), "^\"[^\"]*\""),
                new TokenDefinition(nameof(ExpressionTokenType.OpenParenthesis), "^\\("),
                new TokenDefinition(nameof(ExpressionTokenType.CloseParenthesis), "^\\)"),
                new TokenDefinition(nameof(ExpressionTokenType.OpenBracket), "^\\["),
                new TokenDefinition(nameof(ExpressionTokenType.CloseBracket), "^\\]"),
                new TokenDefinition(nameof(ExpressionTokenType.Comma), "^,"),
                new TokenDefinition(nameof(ExpressionTokenType.Dot), "^\\."),
                new TokenDefinition(nameof(ExpressionTokenType.AndFunction), "^and", true),
                new TokenDefinition(nameof(ExpressionTokenType.OrFunction), "^or", true),
                new TokenDefinition(nameof(ExpressionTokenType.NotFunction), "^not", true),
                new TokenDefinition(nameof(ExpressionTokenType.XorFunction), "^xor", true),
                new TokenDefinition(nameof(ExpressionTokenType.EqualFunction), "^eq", true),
                new TokenDefinition(nameof(ExpressionTokenType.NotEqualFunction), "^ne", true),
                new TokenDefinition(nameof(ExpressionTokenType.Format), "^format", true),
                new TokenDefinition(nameof(ExpressionTokenType.GreaterOrEqualFunction),  "^ge", true),
                new TokenDefinition(nameof(ExpressionTokenType.GreaterThanFunction),  "^gt", true),
                new TokenDefinition(nameof(ExpressionTokenType.In), "^in", true),
                new TokenDefinition(nameof(ExpressionTokenType.NotIn), "^notIn", true),
                new TokenDefinition(nameof(ExpressionTokenType.Join), "^join", true),
                new TokenDefinition(nameof(ExpressionTokenType.LessOrEqualFunction), "^le", true),
                new TokenDefinition(nameof(ExpressionTokenType.LessThanFunction), "^lt", true),
                new TokenDefinition(nameof(ExpressionTokenType.StartsWith), "^startsWith", true),
                new TokenDefinition(nameof(ExpressionTokenType.EndsWith), "^endsWith", true),
                new TokenDefinition(nameof(ExpressionTokenType.Variables), "^variables", true),
                new TokenDefinition(nameof(ExpressionTokenType.Parameters), "^parameters", true),
                new TokenDefinition(nameof(ExpressionTokenType.Providers), "^providers", true),
            })
        {
            IgnoreLineBreaks = true;
            IgnoreWhitespaces = true;
            AddSequenceTerminatorToken = false;
        }

        protected override string GetUnknownTokenType()
        {
            return nameof(ExpressionTokenType.Unknown);
        }

        protected override string GetStringTokenType()
        {
            return nameof(ExpressionTokenType.StringValue);
        }
    }
}
