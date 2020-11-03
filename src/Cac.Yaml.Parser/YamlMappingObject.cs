using Cac.Exceptions;
using System.Collections.Generic;
using System.Text;

namespace Cac.Yaml
{
    public class YamlMappingObject : YamlObject
    {
        public YamlMappingObject() : base(YamlObjectType.Mapping)
        {
        }

        public IYamlObject Value { get; set; }

        public Dictionary<string, IYamlObject> Children { get; } = new Dictionary<string, IYamlObject>();

        public override bool IsEmpty => (Value == null || Value.IsEmpty) && Children.Count == 0;

        public void ChildrenAdd(string key, IYamlObject yaml)
        {
            if (Children.ContainsKey(key))
            {
                throw new UnexpectedCharacterException($"The mapping already contains '{key}' property", yaml.Line, yaml.Column, null);
            }

            Children.Add(key, yaml);
        }

        public override object Clone()
        {
            var result = new YamlMappingObject
            {
                Value = Value != null ? (IYamlObject)Value.Clone() : null,
                Line = Line,
                Column = Column
            };

            foreach (var kvp in Children)
            {
                result.ChildrenAdd(kvp.Key, (IYamlObject)kvp.Value.Clone());
            }

            return result;
        }

        public override string ToString(int spaces, bool isSequence, bool isMapping)
        {
            var addLine = !isSequence && isMapping;
            var space = new string(' ', spaces);
            var sb = new StringBuilder();
            if (Value != null && !Value.IsEmpty)
            {
                sb.Append(Value.ToString(0, false, true));
                sb.Append("\n");
                addLine = false;
            }

            if (addLine)
            {
                sb.Append("\n");
            }

            foreach (var child in Children)
            {
                if (addLine)
                {
                    sb.Append(space);
                }

                addLine = true;
                sb.Append(child.Key);
                sb.Append(": ");
                sb.Append(child.Value.ToString(spaces + 2, false, true));
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
