using System;

namespace Cac.Exceptions
{
    public class UnexpectedBeginOfLineException : Exception
    {
        private const string UnexpectedBeginOfLineMessage = "Line or spacing not expected";

        public UnexpectedBeginOfLineException(int line, string value) : this(line, value, default)
        {
        }

        public UnexpectedBeginOfLineException(int line, string value, Exception innerException) : base(UnexpectedBeginOfLineMessage, innerException)
        {
            Line = line;
            Value = value;
        }

        public int Line { get; }

        public string Value { get; }
    }
}
