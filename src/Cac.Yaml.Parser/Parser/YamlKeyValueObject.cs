namespace Cac.Yaml.Parser
{
    public class YamlKeyValueObject : YamlObject
    {
        protected YamlKeyValueObject() : base(YamlObjectType.Mapping)
        {

        }

        public YamlKeyValueObject(string key) : this()
        {
            Key = key;
        }

        public YamlKeyValueObject(string key, IYamlObject value) : this(key)
        {
            Value = value;
        }

        public string Key { get;  }

        public IYamlObject Value { get; set; }

        public override bool IsEmpty => Value == null || Value.IsEmpty;

        public override string ToString(int spaces, bool isSequence, bool isMapping)
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            return new YamlKeyValueObject(Key, (IYamlObject)Value.Clone()) { Line = Line, Column = Column }; 
        }
    }
}
