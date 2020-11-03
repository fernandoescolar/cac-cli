using Cac.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cac.Yaml.Mapping
{
    public static class YamlMappingObjectExtensions
    {
        public static T Map<T>(this YamlMappingObject yaml)
        {
            var result = yaml.Map(typeof(T));
            return result is T t ? t : default;
        }

        public static object Map(this YamlMappingObject yaml, Type type)
        {
            if (yaml.TryMap(type, out var o))
            {
                return o;
            }

            throw new UnexpectedTypeException($"Can not map '{type.Name}' from yaml mapping object", yaml.Line, yaml.Column);
        }

        public static bool TryMap<T>(this YamlMappingObject yaml, out T o)
        {
            if (yaml.TryMap(typeof(T), out var result))
            {
                o = result == null ? default : (T)result;
                return true;
            }

            o = default;
            return false;
        }

        public static bool TryMap(this YamlMappingObject yaml, Type type, out object o)
        {
            if (type.FullName == typeof(object).FullName)
            {
                o = yaml;
                return true;
            }

            if (type.Name == typeof(Dictionary<,>).Name)
            {

                return yaml.TryMapAsDictionary(type, out o);
            }

            return yaml.TryMapAsObject(type, out o);
        }

        public static T MapProperty<T>(this YamlMappingObject yaml, string name)
        {
            if (!yaml.Children.ContainsKey(name)) throw new PropertyNotFoundException(name, yaml.Line, yaml.Column);
            if (yaml.Children[name] is YamlScalarObject o) return o.Map<T>();
            throw new UnexpectedTypeException($"Property '{name}' expected to be a scalar, found `{yaml.Children[name].GetType().Name}`", yaml.Children[name].Line, yaml.Children[name].Column);
        }

        public static IEnumerable<T> MapPropertyAsEnumerable<T>(this YamlMappingObject yaml, string name)
        {
            if (!yaml.Children.ContainsKey(name)) throw new PropertyNotFoundException(name, yaml.Line, yaml.Column);
            if (yaml.Children[name] is YamlSequenceObject o) return o.Map<IEnumerable<T>>();
            throw new UnexpectedTypeException($"Property '{name}' expected to be a sequence, found `{yaml.Children[name].GetType().Name}`", yaml.Children[name].Line, yaml.Children[name].Column);
        }

        private static bool TryMapAsDictionary(this YamlMappingObject yaml, Type type, out object o)
        {
            if (type.GetGenericArguments()[0].FullName != typeof(string).FullName && type.GetGenericArguments()[0].FullName != typeof(object).FullName)
            {
                o = default;
                return false;
            }

            var elementType = type.GetGenericArguments()[1];
            var diccionary = (IDictionary)Activator.CreateInstance(type);
            foreach (var child in yaml.Children)
            {
                child.Value.TryMap(elementType, out var value);
                diccionary.Add(child.Key, value);
            }

            o = diccionary;
            return true;
        }

        private static bool TryMapAsObject(this YamlMappingObject yaml, Type type, out object o)
        {
            var constructor = type.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0);
            if (constructor == null)
            {
                o = default;
                return false;
            }

            var result = constructor.Invoke(new object[0]);
            type.GetProperties()
                .Where(x => x.GetCustomAttribute<YamlPropertyAttribute>() != null)
                .Select(x => new { YamlProperty = x.GetCustomAttribute<YamlPropertyAttribute>(), Type = x.PropertyType, PropertyInfo = x,  })
                .ToList()
                .ForEach(x =>
                {
                    if (yaml.Children.ContainsKey(x.YamlProperty.Name) && yaml.Children[x.YamlProperty.Name].TryMap(x.Type, out var o)) x.PropertyInfo.SetValue(result, o);
                    else if (x.YamlProperty.IsRequired) throw new PropertyNotFoundException(x.YamlProperty.Name, yaml.Line, yaml.Column);
                });
            type.GetFields()
                .Where(x => x.GetCustomAttribute<YamlPropertyAttribute>() != null)
                .Select(x => new { YamlProperty = x.GetCustomAttribute<YamlPropertyAttribute>(), Type = x.FieldType, FieldInfo = x })
                .ToList()
                .ForEach(x =>
                {
                    if (yaml.Children.ContainsKey(x.YamlProperty.Name) && yaml.Children[x.YamlProperty.Name].TryMap(x.Type, out var o)) x.FieldInfo.SetValue(result, o);
                    else if (x.YamlProperty.IsRequired) throw new PropertyNotFoundException(x.YamlProperty.Name, yaml.Line, yaml.Column);
                });

            o = result;
            return true;
        }
    }
}
