using System;

namespace Cac.Exceptions
{
    public class UnexpectedTypeException : Exception
    {
        public UnexpectedTypeException(string message, int line, int column) : this(message, line, column, default)
        {
        }

        public UnexpectedTypeException(string message, int line, int column, string value) : this(message, line, column, value, default)
        {
        }

        public UnexpectedTypeException(string message, int line, int column, string value, Exception innerException) : base(message, innerException)
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
