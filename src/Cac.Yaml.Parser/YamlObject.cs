namespace Cac.Yaml
{
    public abstract class YamlObject : IYamlObject
    {
        public YamlObject(YamlObjectType type)
        {
            YamlObjectType = type;
        }

        public YamlObjectType YamlObjectType { get; protected set; }

        public abstract bool IsEmpty { get; }

        public int Line { get; set; }

        public int Column { get; set; }

        public abstract object Clone();

        public abstract string ToString(int spaces, bool isSequence, bool isMapping);
        
        public override string ToString()
        {
            var str = ToString(0, false, false);
            while (str.Contains("\n\n"))
                str = str.Replace("\n\n", "\n");

            return str;
        }
    }
}
