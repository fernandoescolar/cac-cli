using Cac.Tokenization;
using System.Collections.Generic;

namespace Cac.Yaml.Parser
{
    public class YamlTokenizer : AbstractTokenizer
    {
        public YamlTokenizer() :
            base(new List<TokenDefinition> {
                new TokenDefinition(nameof(YamlTokenType.Expression), "^\\${{[^}}]*}}"),
                new TokenDefinition(nameof(YamlTokenType.Space), "^\\x20+"),
                new TokenDefinition(nameof(YamlTokenType.Space), "^\\t+"),
                new TokenDefinition(nameof(YamlTokenType.Version), "^\\d+\\.\\d+\\.\\d+(\\.\\d+)?"),
                new TokenDefinition(nameof(YamlTokenType.Boolean), "^true|false"),
                new TokenDefinition(nameof(YamlTokenType.Integer), "^\\d+"),
                new TokenDefinition(nameof(YamlTokenType.Integer), "^0x[a-fA-F0-9]+"),
                new TokenDefinition(nameof(YamlTokenType.Float), "^\\d+\\.\\d+"),
                new TokenDefinition(nameof(YamlTokenType.StringValue), "^'[^']*'"),
                new TokenDefinition(nameof(YamlTokenType.StringValue), "^\"[^\"]*\""),
                new TokenDefinition(nameof(YamlTokenType.Colon), "^:"),
                new TokenDefinition(nameof(YamlTokenType.OpenBrace), "^\\{"),
                new TokenDefinition(nameof(YamlTokenType.CloseBrace), "^\\}"),
                new TokenDefinition(nameof(YamlTokenType.OpenBracket), "^\\["),
                new TokenDefinition(nameof(YamlTokenType.CloseBracket), "^\\]"),
                new TokenDefinition(nameof(YamlTokenType.Comma), "^,"),
                new TokenDefinition(nameof(YamlTokenType.Comment), "^#.*\n?"),
                new TokenDefinition(nameof(YamlTokenType.Pipe), "^\\|-?"),
                new TokenDefinition(nameof(YamlTokenType.GreaterThan), "^>-?"),
                new TokenDefinition(nameof(YamlTokenType.Dash), "^-"),
                new TokenDefinition(nameof(YamlTokenType.Null), "^null"),
                new TokenDefinition(nameof(YamlTokenType.Null), "^~"),
                new TokenDefinition(nameof(YamlTokenType.DateTime), "^\\d\\d\\d\\d-\\d\\d-\\d\\d"),
                new TokenDefinition(nameof(YamlTokenType.DateTime), "^\\d\\d\\d\\d-\\d\\d-\\d\\dT\\d\\d:\\d\\d:\\d\\d"),
                // new TokenDefinition(nameof(YamlTokenType.Infinity), "^.inf"),
                // new TokenDefinition(nameof(YamlTokenType.ConvertToString), "^!!str"),
                // new TokenDefinition(nameof(YamlTokenType.ConvertToFloat), "^!!float"),
                // new TokenDefinition(nameof(YamlTokenType.ConvertToBinary), "^!!binary"),
                
            })
        {
            IgnoreLineBreaks = false;
            IgnoreWhitespaces = true;
        }

        protected override string GetSequenceTerminatorTokenType()
        {
            return nameof(YamlTokenType.SequenceTerminator);
        }

        protected override string GetUnknownTokenType()
        {
            return nameof(YamlTokenType.Unknown);
        }

        protected override string GetLineBreakTokenType()
        {
            return nameof(YamlTokenType.LineBreak);
        }

        protected override string GetStringTokenType()
        {
            return nameof(YamlTokenType.StringValue);
        }

        protected override string GetUnknownTokenDelimiters()
        {
            return " :,;{}[]()~\n\r#\"'";
        }
    }
}
