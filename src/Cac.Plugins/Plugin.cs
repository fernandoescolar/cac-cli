using Cac.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cac.Plugins
{
    public class Plugin
    {
        private readonly Assembly _assembly;
        private readonly string[] _imports;

        public Plugin(Assembly assembly, string name, string version, string[] imports = null)
        {
            _assembly = assembly;
            Name = name;
            Version = version;
            _imports = imports;
        }

        public string Name { get; }

        public string Version { get; }

        public IDictionary<string, ICacActivity> CreateActivities()
        {
            return _assembly.GetNamedTypesOf<ICacActivity>()
                            .Where(x => _imports == null || !_imports.Any() || _imports.Contains(x.Key))
                            .ToDictionary(x => x.Key, x => Activator.CreateInstance(x.Value) as ICacActivity);
        }

        public IDictionary<string, ICacProvider> CreateProviders()
        {
            return _assembly.GetNamedTypesOf<ICacProvider>()
                            .Where(x => _imports == null || !_imports.Any() || _imports.Contains(x.Key))
                            .ToDictionary(x => x.Key, x => Activator.CreateInstance(x.Value) as ICacProvider);
        }

        public IEnumerable<ICacCommandHandler> CreateCommandHandlers()
        {
            return _assembly.GetTypes()
                            .Where(type => !type.IsAbstract && typeof(ICacCommandHandler).IsAssignableFrom(type))
                            .Select(x => (ICacCommandHandler)Activator.CreateInstance(x))
                            .ToList();
        }
    }
}
