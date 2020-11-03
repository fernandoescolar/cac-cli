using Cac.Yaml.Mapping;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Cac.Yaml.Conversion
{
    public class YamlConverter : IYamlConverter
    {
        public IYamlObject From(object o)
        {
            if (o is IYamlObject yaml) return yaml;
            if (o == null) return new YamlScalarObject();
            if (o is bool b) return new YamlScalarObject(b);
            if (o is short s) return new YamlScalarObject(s);
            if (o is int i) return new YamlScalarObject(i);
            if (o is long l) return new YamlScalarObject(l);
            if (o is float f) return new YamlScalarObject(f);
            if (o is double d) return new YamlScalarObject(d);
            if (o is decimal dec) return new YamlScalarObject(dec);
            if (o is string str) return new YamlScalarObject(str);
            if (o is DateTime dateTime) return new YamlScalarObject(dateTime);
            if (o is Version version) return new YamlScalarObject(version);
            if (o is IDictionary dic)
            {
                var m = new YamlMappingObject();
                foreach (string key in dic.Keys)
                {
                    m.ChildrenAdd(key, From(dic[key]));
                }

                return m;
            }
            if (o is IEnumerable e)
            {
                var sequence = new YamlSequenceObject();
                foreach (var item in e)
                {
                    sequence.Add(From(item));
                }

                return sequence;
            }

            var propertiesDictionary = o.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(x => x.GetCustomAttribute<YamlPropertyAttribute>() != null)
                                        .ToDictionary(x => x.GetCustomAttribute<YamlPropertyAttribute>().Name, x => x.GetValue(o));
            var fieldsDictionary = o.GetType().GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(x => x.GetCustomAttribute<YamlPropertyAttribute>() != null)
                                    .ToDictionary(x => x.GetCustomAttribute<YamlPropertyAttribute>().Name, x => x.GetValue(o));

            var mergedDictionary = new[] { propertiesDictionary, fieldsDictionary }.SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value);
            return From(mergedDictionary);
        }

        public T To<T>(IYamlObject yaml)
        {
            if (yaml is YamlScalarObject c && c.TryMap<T>(out var n)) return n;
            if (yaml is YamlSequenceObject s && s.TryMap<T>(out var e)) return e;
            if (yaml is YamlMappingObject m && m.TryMap<T>(out var o)) return o;

            return default;
        }
    }
}
