using System;

namespace Cac.Exceptions
{
    public class ExpressionEvaluationException : Exception
    {
        public ExpressionEvaluationException(int line, int column, Exception innerException) : base(innerException.Message, innerException)
        {
            Line = line;
            Column = column;
        }


        public int Line { get; }

        public int Column { get; }
    }
}
