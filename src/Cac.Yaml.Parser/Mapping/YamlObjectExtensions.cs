using Cac.Exceptions;
using System;

namespace Cac.Yaml.Mapping
{
    public static class YamlObjectExtensions
    {
        public static T Map<T>(this IYamlObject yaml) 
        {
            return yaml switch
            {
                YamlScalarObject c => c.Map<T>(),
                YamlSequenceObject s => s.Map<T>(),
                YamlMappingObject m => m.Map<T>(),
                _ => throw new UnexpectedTypeException($"Yaml deserialization: object '{yaml.GetType().Name}' is not supported", yaml.Line, yaml.Column)
            };
        }

        public static object Map(this IYamlObject yaml, Type type)
        {
            return yaml switch
            {
                YamlScalarObject c => c.Map(type),
                YamlSequenceObject s => s.Map(type),
                YamlMappingObject m => m.Map(type),
                _ => throw new UnexpectedTypeException($"Yaml deserialization: object '{yaml.GetType().Name}' is not supported", yaml.Line, yaml.Column)
            };
        }

        public static bool TryMap<T>(this IYamlObject yaml, out T o)
        {
            return yaml switch
            {
                YamlScalarObject c => c.TryMap<T>(out o),
                YamlSequenceObject s => s.TryMap<T>(out o),
                YamlMappingObject m => m.TryMap<T>(out o),
                _ => throw new UnexpectedTypeException($"Yaml deserialization: object '{yaml.GetType().Name}' is not supported", yaml.Line, yaml.Column)
            };
        }

        public static bool TryMap(this IYamlObject yaml, Type type, out object o)
        {
            return yaml switch
            {
                YamlScalarObject c => c.TryMap(type, out o),
                YamlSequenceObject s => s.TryMap(type, out o),
                YamlMappingObject m => m.TryMap(type, out o),
                _ => throw new UnexpectedTypeException($"Yaml deserialization: object '{yaml.GetType().Name}' is not supported", yaml.Line, yaml.Column)
            };
        }
    }
}
