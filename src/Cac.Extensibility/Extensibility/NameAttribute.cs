using System;

namespace Cac.Extensibility
{
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
