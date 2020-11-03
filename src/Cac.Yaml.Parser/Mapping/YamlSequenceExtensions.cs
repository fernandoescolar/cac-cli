using Cac.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cac.Yaml.Mapping
{
    public static class YamlSequenceExtensions
    {
        public static T Map<T>(this YamlSequenceObject yaml)
        {
            var result = yaml.Map(typeof(T));
            return result is T t ? t : default;
        }

        public static object Map(this YamlSequenceObject yaml, Type type)
        {
            if (yaml.TryMap(type, out var o))
            {
                return o;
            }

            throw new UnexpectedTypeException($"Can not map '{type.Name}' from yaml sequence object", yaml.Line, yaml.Column);
        }

        public static bool TryMap<T>(this YamlSequenceObject yaml, out T o)
        {
            var type = typeof(T);
            if (yaml.TryMap(type, out var result))
            {
                o = (T)result;
                return true;
            }

            o = default;
            return false;
        }

        public static bool TryMap(this YamlSequenceObject yaml, Type type, out object o)
        {
            if (type.FullName == typeof(object).FullName)
            {
                o = yaml;
                return true;
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var enumerable = yaml.MapAsEnumerable(elementType);
                var result = Activator.CreateInstance(type, new object[] { yaml.Count() });
                var array = (Array)result;
                var counter = 0;
                foreach (var item in enumerable)
                {
                    array.SetValue(item, counter++);
                }

                o = result;
                return true;

            }

            if (type.Name == typeof(IEnumerable<>).Name
                || type.Name == typeof(IList<>).Name
                || type.Name == typeof(ICollection<>).Name
                || type.Name == typeof(List<>).Name)
            {
                var elementType = type.GetGenericArguments()[0];
                o = yaml.MapAsList(elementType);
                return true;
            }

            o = default;
            return false;
        }

        private static IList MapAsList(this YamlSequenceObject yaml, Type elementType)
        {
            var type = typeof(List<>);
            type.MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(type);
            foreach (var item in yaml.MapAsEnumerable(elementType))
            {
                list.Add(item);
            }

            return list;
        }

        private static IEnumerable MapAsEnumerable(this YamlSequenceObject yaml, Type elementType)
        {
            foreach (var child in yaml)
            {
                yield return child.Map(elementType);
            }
        }
    }
}
