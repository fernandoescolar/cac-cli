using Cac.Exceptions;
using System;
using System.Globalization;

namespace Cac.Yaml.Mapping
{
    public static class YamlScalarObjectExtensions
    {
        public static T Map<T>(this YamlScalarObject yaml)
        {
            var result = yaml.Map(typeof(T));
            return result is T t ? t : default;
        }

        public static object Map(this YamlScalarObject yaml, Type type)
        {
            if (yaml.TryMap(type, out var o))
            {
                return o;
            }

            throw new UnexpectedTypeException($"Yaml mapping: can not assign value from {yaml.YamlScalarType} to {type.Name}.", yaml.Line, yaml.Column);
        }

        public static bool TryMap<T>(this YamlScalarObject yaml, out T o)
        {
            if (yaml.TryMap(typeof(T), out var result))
            {
                o = result == null ? default : (T)result;
                return true;
            }

            o = default;
            return false;
        }

        public static bool TryMap(this YamlScalarObject yaml, Type type, out object o)
        {
            if (type.FullName == typeof(object).FullName) return yaml.TryMapAsObject(out o);
            if (yaml.YamlScalarType == YamlScalarType.Null && type.IsNullable()) { o = null; return true; }
            if (type.Is<bool>() && yaml.YamlScalarType == YamlScalarType.Boolean) { o = bool.TryParse(yaml.Value, out var result) && result; return true; }
            if (type.Is<short>() && yaml.YamlScalarType == YamlScalarType.Int) { o = short.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (type.Is<int>() && yaml.YamlScalarType == YamlScalarType.Int) { o = int.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (type.Is<long>() && yaml.YamlScalarType == YamlScalarType.Int) { o = long.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (type.Is<float>() && yaml.YamlScalarType == YamlScalarType.Float) { o = float.TryParse(yaml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default; return true; }
            if (type.Is<double>() && yaml.YamlScalarType == YamlScalarType.Float) { o = double.TryParse(yaml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default; return true; }
            if (type.Is<decimal>() && yaml.YamlScalarType == YamlScalarType.Float) { o = decimal.TryParse(yaml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default; return true; }
            if (type.Is<DateTime>() && yaml.YamlScalarType == YamlScalarType.DateTime) { o = DateTime.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (type.Is<Version>() && yaml.YamlScalarType == YamlScalarType.Version) { o = Version.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (type.Is<string>()) { o = yaml.Value; return true; }

            o = default;
            return false;
        }

        private static bool TryMapAsObject(this YamlScalarObject yaml, out object o)
        {
            if (yaml.YamlScalarType == YamlScalarType.Null) { o = null; return true; }
            if (yaml.YamlScalarType == YamlScalarType.Boolean) { o = bool.TryParse(yaml.Value, out var result) && result; return true; }
            if (yaml.YamlScalarType == YamlScalarType.Int) { o = int.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (yaml.YamlScalarType == YamlScalarType.Float) { o = float.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (yaml.YamlScalarType == YamlScalarType.String) { o = yaml.Value; return true; }
            if (yaml.YamlScalarType == YamlScalarType.DateTime) { o = DateTime.TryParse(yaml.Value, out var result) ? result : default; return true; }
            if (yaml.YamlScalarType == YamlScalarType.Version) { o = Version.TryParse(yaml.Value, out var result) ? result : default; return true; }

            o = default;
            return false;
        }

        private static bool Is<T>(this Type @this)
        {
            return @this.IsAssignableFrom(typeof(T));
        }

        private static bool IsNullable(this Type @this)
        {
            return Nullable.GetUnderlyingType(@this) != null;
        }
    }
}
