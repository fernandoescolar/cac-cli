using Cac.Tokenization;
using System.Collections.Generic;

namespace Cac.Expressions.Parser
{
    public class ExpressionsTokenizer : AbstractTokenizer
    {
        public ExpressionsTokenizer() :
            base(new List<TokenDefinition> {
                new TokenDefinition(nameof(ExpressionTokenType.Null), "^null"),
                new TokenDefinition(nameof(ExpressionTokenType.Boolean), "^true|false"),
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
                new TokenDefinition(nameof(ExpressionTokenType.AndFunction), "^and"),
                new TokenDefinition(nameof(ExpressionTokenType.OrFunction), "^or"),
                new TokenDefinition(nameof(ExpressionTokenType.NotFunction), "^not"),
                new TokenDefinition(nameof(ExpressionTokenType.XorFunction), "^xor"),
                new TokenDefinition(nameof(ExpressionTokenType.EqualFunction), "^eq"),
                new TokenDefinition(nameof(ExpressionTokenType.NotEqualFunction), "^ne"),
                new TokenDefinition(nameof(ExpressionTokenType.Format), "^format"),
                new TokenDefinition(nameof(ExpressionTokenType.GreaterOrEqualFunction),  "^ge"),
                new TokenDefinition(nameof(ExpressionTokenType.GreaterThanFunction),  "^gt"),
                new TokenDefinition(nameof(ExpressionTokenType.In), "^in"),
                new TokenDefinition(nameof(ExpressionTokenType.NotIn), "^notIn"),
                new TokenDefinition(nameof(ExpressionTokenType.Join), "^join"),
                new TokenDefinition(nameof(ExpressionTokenType.LessOrEqualFunction), "^le"),
                new TokenDefinition(nameof(ExpressionTokenType.LessThanFunction), "^lt"),
                new TokenDefinition(nameof(ExpressionTokenType.StartsWith), "^startsWith"),
                new TokenDefinition(nameof(ExpressionTokenType.EndsWith), "^endsWith"),
                new TokenDefinition(nameof(ExpressionTokenType.Variables), "^variables"),
                new TokenDefinition(nameof(ExpressionTokenType.Parameters), "^parameters"),
                new TokenDefinition(nameof(ExpressionTokenType.Providers), "^providers"),
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
