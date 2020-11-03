using System;

namespace Cac.Yaml
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YamlPropertyAttribute : Attribute
    {
        public YamlPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool IsRequired { get; set; }
    }
}
