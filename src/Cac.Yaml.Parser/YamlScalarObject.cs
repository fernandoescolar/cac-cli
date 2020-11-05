using System;
using System.Globalization;

namespace Cac.Yaml
{
    public class YamlScalarObject : YamlObject
    {
        public YamlScalarObject() : this(null, YamlScalarType.Null)
        {
        }

        public YamlScalarObject(bool value) : this(value.ToString(), YamlScalarType.Boolean)
        {
        }

        public YamlScalarObject(short value) : this(value.ToString(), YamlScalarType.Int)
        {
        }

        public YamlScalarObject(int value) : this(value.ToString(), YamlScalarType.Int)
        {
        }

        public YamlScalarObject(long value) : this(value.ToString(), YamlScalarType.Int)
        {
        }

        public YamlScalarObject(float value) : this(value.ToString("G", CultureInfo.InvariantCulture), YamlScalarType.Float)
        {
        }

        public YamlScalarObject(double value) : this(value.ToString("G", CultureInfo.InvariantCulture), YamlScalarType.Float)
        {
        }

        public YamlScalarObject(decimal value) : this(value.ToString("G", CultureInfo.InvariantCulture), YamlScalarType.Float)
        {
        }

        public YamlScalarObject(string value) : this(value, YamlScalarType.String)
        {
        }

        public YamlScalarObject(DateTime value) : this(value.ToString(), YamlScalarType.DateTime)
        {
        }

        public YamlScalarObject(Version value) : this(value.ToString(), YamlScalarType.Version)
        {
        }

        public YamlScalarObject(string value, YamlScalarType type) : base(YamlObjectType.Scalar)
        {
            Value = value;
            YamlScalarType = type;
        }

        public string Value { get; set; }

        public YamlScalarType YamlScalarType { get; set; }

        public override bool IsEmpty => string.IsNullOrEmpty(Value) || YamlScalarType == YamlScalarType.Null;

        public override object Clone()
        {
            return new YamlScalarObject(Value, YamlScalarType) { Line = Line, Column = Column };
        }

        public override string ToString(int spaces, bool isSequence, bool isMapping)
        {
            var space = isSequence || isMapping ? string.Empty : new string(' ', spaces);
            return $"{space}{Value}";
        }
    }
}
