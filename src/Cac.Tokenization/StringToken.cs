namespace Cac.Tokenization
{
    public class StringToken : Token
    {
        public StringToken(string tokenType, string value, int line, int column) : base(tokenType, value, line, column)
        {
        }

        public override string Value { get => base.Value; set => base.Value = FormatFromStringValueToken(value); }

        private static string FormatFromStringValueToken(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            var c = str[0];
            if (str.StartsWith(@"""") || str.StartsWith("'"))
            {
                str = str.Substring(1, str.Length - 2);
            }

            str = str.Replace($"{c}{c}", $"{c}");
            str = str.Replace("\\a", "\a");
            str = str.Replace("\\b", "\b");
            str = str.Replace("\\f", "\f");
            str = str.Replace("\\n", "\n");
            str = str.Replace("\\t", "\t");
            str = str.Replace("\\r", "\r");
            str = str.Replace("\\v", "\v");
            str = str.Replace("\\0", "\0");
            str = str.Replace("\\x01", "\x01");
            str = str.Replace("\\x02", "\x02");
            str = str.Replace("\\x03", "\x03");
            str = str.Replace("\\x04", "\x04");
            str = str.Replace("\\x05", "\x05");
            str = str.Replace("\\x06", "\x06");
            str = str.Replace("\\x07", "\x07");
            str = str.Replace("\\x08", "\x08");
            str = str.Replace("\\x09", "\x09");
            str = str.Replace("\\x0a", "\x0a");
            str = str.Replace("\\x0b", "\x0b");
            str = str.Replace("\\x0c", "\x0c");
            str = str.Replace("\\x0d", "\x0d");
            str = str.Replace("\\x0e", "\x0e");
            str = str.Replace("\\x0f", "\x0f");
            str = str.Replace("\\x10", "\x10");
            str = str.Replace("\\x11", "\x11");
            str = str.Replace("\\x12", "\x12");
            str = str.Replace("\\x13", "\x13");
            str = str.Replace("\\x14", "\x14");
            str = str.Replace("\\x15", "\x15");
            str = str.Replace("\\x16", "\x16");
            str = str.Replace("\\x17", "\x17");
            str = str.Replace("\\x18", "\x18");
            str = str.Replace("\\x19", "\x19");
            str = str.Replace("\\x1a", "\x1a");
            str = str.Replace("\\x1b", "\x1b");
            str = str.Replace("\\x1c", "\x1c");
            str = str.Replace("\\x1d", "\x1d");
            str = str.Replace("\\x1e", "\x1e");
            str = str.Replace("\\x1f", "\x1f");
            return str;
        }
    }
}
