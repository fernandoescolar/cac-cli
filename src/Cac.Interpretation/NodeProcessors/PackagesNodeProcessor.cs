using Cac.Exceptions;
using Cac.Plugins;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System;
using System.IO;
using System.Linq;

namespace Cac.Interpretation.NodeProcessors
{
    public class PackagesNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "packages".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            var sequence = node as YamlSequenceObject ?? throw new UnexpectedTypeException("Yaml node should be a sequence", node.Line, node.Column);
            foreach (var entry in sequence)
            {
                context.ExpressionEvaluator.EvaluateExpressions(entry);
                if (entry is YamlMappingObject m)
                {
                    string[] imports = null;
                    if (m.Children.ContainsKey("file"))
                    {
                        var file = m.MapProperty<string>("file");
                        if (m.Children.ContainsKey("import"))
                        {
                            imports = m.MapPropertyAsEnumerable<string>("import").ToArray();
                        }

                        LoadPackage(context, file, imports);
                    }
                    else
                    {
                        var package = m.MapProperty<string>("name");
                        var version = m.MapProperty<string>("version");
                        if (m.Children.ContainsKey("import"))
                        {
                            imports = m.MapPropertyAsEnumerable<string>("import").ToArray();
                        }

                        LoadPackage(context, package, version, imports);
                    }
                }
            }
        }

        private static void LoadPackage(IExecutionContext context, string file, string[] imports)
        {
            var plugin = ReadPackage(context, file, imports);
            LoadPlugin(context, (Plugin)plugin);
        }

        private static void LoadPackage(IExecutionContext context, string package, string version, string[] imports)
        {
            var plugin = DownloadPackage(context, package, version, imports);
            LoadPlugin(context, plugin);
        }

        private static Plugin ReadPackage(IExecutionContext context, string file, string[] imports)
        {
            if (!File.Exists(file)) throw new FileNotFoundException($"Package file not found: {file}");

            context.Out.WriteLine($"Loading: {file}");
            return context.PluginManager.LoadPlugin(file, imports);
        }

        private static Plugin DownloadPackage(IExecutionContext context, string package, string version, string[] imports)
        {
            context.Out.WriteLine($"Downloading: {package} v{version}");
            return context.PluginManager.LoadPlugin(package, version, imports);
        }

        private static void LoadPlugin(IExecutionContext context, Plugin plugin)
        {
            var activities = plugin.CreateActivities();
            var providers = plugin.CreateProviders();
            var commandHandlers = plugin.CreateCommandHandlers();
            
            foreach (var activity in activities)
            {
                if (context.AvailableActivities.ContainsKey(activity.Key))
                {
                    context.Out.Warning($"Replacing activity '{activity.Key}' with {plugin.Name}({plugin.Version})");
                }
                else
                {
                    context.Out.Verbose.WriteLine($"Adding activity {activity.Key}", ConsoleColor.Cyan);
                }

                context.AvailableActivities[activity.Key] = activity.Value;
            }

            foreach (var provider in providers)
            {
                if (context.AvailableProviders.ContainsKey(provider.Key))
                {
                    context.Out.Warning($"Replacing provider '{provider.Key}' with {plugin.Name}({plugin.Version})");
                }
                else
                {
                    context.Out.Verbose.WriteLine($"Adding provider {provider.Key}", ConsoleColor.Cyan);
                }

                context.AvailableProviders[provider.Key] = provider.Value;
            }

            context.AvailableCommandHandlers.AddRange(commandHandlers);
        }
    }
}
