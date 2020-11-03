using Cac.Exceptions;
using Cac.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cac.Plugins
{
    public class PluginManager
    {
        private const string PackagesFolder = "./.packages";
        private readonly PackageManager _packageManager;
        private readonly List<Plugin> _plugins;

        public PluginManager(IOutput output)
        {
            _packageManager = new PackageManager(PackagesFolder, output);
            _plugins = new List<Plugin>();
        }

        public Plugin LoadPlugin(string name, string version, string[] imports = null)
        {
            if (!Directory.Exists(PackagesFolder)) Directory.CreateDirectory(PackagesFolder);

            var pluginLocation = InstallPackage(name, version);
            return LoadPlugin(pluginLocation);
        }

        public Plugin LoadPlugin(string filepath, string[] imports = null)
        {
            var loadContext = new PluginLoadContext(filepath);
            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(filepath)));
            var plugin = new Plugin(assembly, assembly.GetName().Name, assembly.GetName().Version.ToString(), imports);

            _plugins.Add(plugin);

            return plugin;
        }

        private string InstallPackage(string packageId, string version)
        {
            try
            {
                var task = _packageManager.InstallAsync(packageId, version);
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                throw new CanNotDownloadPackageException(packageId, version, ex);
            }
        }
    }
}
