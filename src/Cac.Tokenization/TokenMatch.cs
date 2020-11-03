namespace Cac.Tokenization
{
    public class TokenMatch
    {
        public bool IsMatch { get; set; }
        public string TokenType { get; set; }
        public string Value { get; set; }
        public string RemainingText { get; set; }
    }
}
