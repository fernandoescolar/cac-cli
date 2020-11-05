using System;

namespace Cac.Exceptions
{
    public class UnexpectedCharacterException : Exception
    {
        private const string UnexpectedCharacterMessage = "Unexpected character found: ";

        public UnexpectedCharacterException(int line, int column, string value) : this(line, column, value, default)
        {
        }

        public UnexpectedCharacterException(string message, int line, int column, string value) : this(message, line, column, value, default)
        {
        }

        public UnexpectedCharacterException(int line, int column, string value, Exception innerException) : this(UnexpectedCharacterMessage + value, line, column, value, innerException)
        {
        }

        public UnexpectedCharacterException(string message, int line, int column, string value, Exception innerException) : base(message, innerException)
        {
            Line = line;
            Column = column;
            Value = value;
        }

        public int Line { get; }

        public int Column { get; }

        public string Value { get; }
    }
}
