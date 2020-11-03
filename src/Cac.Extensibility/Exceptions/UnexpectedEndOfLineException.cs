using System;

namespace Cac.Exceptions
{
    public class UnexpectedEndOfLineException : Exception
    {
        private const string UnexpectedEndOfLineMessage = "Not expected end of line";

        public UnexpectedEndOfLineException(int line) : this(line, default)
        {
        }

        public UnexpectedEndOfLineException(int line, Exception innerException) : base(UnexpectedEndOfLineMessage, innerException)
        {
            Line = line;
        }

        public int Line { get; }
    }
}
