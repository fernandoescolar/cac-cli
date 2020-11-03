using Cac.Extensibility;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        public static Dictionary<string, Type> GetNamedTypesOf<T>(this Assembly assembly)
        { 
            return assembly.GetTypes()
                           .Where(type => !type.IsAbstract && typeof(T).IsAssignableFrom(type))
                           .Select(type => new { Name = type.GetCustomAttributes().OfType<NameAttribute>().FirstOrDefault()?.Name, Type = type })
                           .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                           .ToDictionary(x => x.Name, x => x.Type);
        }
    }
}
