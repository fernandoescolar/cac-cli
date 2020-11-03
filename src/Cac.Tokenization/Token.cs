namespace Cac.Tokenization
{
    public class Token
    {
        public Token(string tokenType, string value, int line, int column)
        {
            TokenType = tokenType;
            Value = value;
            Line = line;
            Column = column;
        }

        public string TokenType { get; set; }

        public virtual string Value { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public override string ToString()
        {
            return $"{Value} <{TokenType}>";
        }
    }
}
