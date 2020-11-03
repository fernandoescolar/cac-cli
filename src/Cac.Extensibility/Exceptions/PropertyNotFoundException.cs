using System;

namespace Cac.Exceptions
{
    public class PropertyNotFoundException : Exception
    {
        private const string PropertyNotFoundMessage = "Property not found"; 

        public PropertyNotFoundException(string propertyName, int line, int column) : this(propertyName, line, column, null)
        {
        }

        public PropertyNotFoundException(string propertyName, int line, int column, Exception innerException) : base($"{PropertyNotFoundMessage}: '{propertyName}'", innerException)
        {
            Line = line;
            Column = column;
            PropertyName = propertyName;
        }


        public int Line { get; }

        public int Column { get; }

        public string PropertyName { get; }
    }
}
