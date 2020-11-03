namespace Cac.Yaml.Parser
{
    public enum YamlTokenType
    {
        Unknown,
        Space,
        SequenceTerminator,
        LineBreak,
        Version,
        Boolean,
        Integer,
        Float,
        StringValue,
        Colon,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Comma,
        Comment,
        Pipe,
        GreaterThan,
        Dash,
        Null,
        DateTime,
        // Infinity,
        // ConvertToString,
        // ConvertToFloat,
        // ConvertToBinary,
        Expression
    }
}
