using Cac.Yaml;
using System;

namespace Cac.Exceptions
{
    public class UnexpectedYamlObjectException : Exception
    {
        public UnexpectedYamlObjectException(string message, IYamlObject yamlObject) : this(message, yamlObject, default)
        {
        }

        public UnexpectedYamlObjectException(string message, IYamlObject yamlObject, Exception innerException) : base(message, innerException)
        {
            YamlObject = yamlObject;
        }

        public int Line { get => YamlObject.Line; }

        public int Column { get => YamlObject.Column; }

        public IYamlObject YamlObject { get; }
    }
}
