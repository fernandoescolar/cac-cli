using Cac.Default.Providers;
using Cac.Expressions;
using Cac.Extensibility;
using Cac.Options;
using Cac.Output;
using Cac.Plugins;
using Cac.Yaml;
using Cac.Yaml.Conversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Cac.Interpretation
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(ICacOptions options, IOutput output)
        {
            Options = options;
            Out = output;
            ExpressionEvaluator = new Evaluator(this);
            AvailableActivities = typeof(JsonProvider)
                                           .Assembly
                                           .GetNamedTypesOf<ICacActivity>()
                                           .ToDictionary(x => x.Key, x => (ICacActivity)Activator.CreateInstance(x.Value));
            AvailableProviders = typeof(JsonProvider)
                                           .Assembly
                                           .GetNamedTypesOf<ICacProvider>()
                                           .ToDictionary(x => x.Key, x => (ICacProvider)Activator.CreateInstance(x.Value));
            AvailableCommandHandlers = typeof(JsonProvider)
                                           .Assembly
                                           .GetTypes()
                                           .Where(type => !type.IsAbstract && typeof(ICacCommandHandler).IsAssignableFrom(type))
                                           .Select(x => (ICacCommandHandler)Activator.CreateInstance(x))
                                           .ToList();
            Commands = new List<ICacCommand>();
            PluginManager = new PluginManager(Out);
            YamlConverter = new YamlConverter();
        }

        public ICacOptions Options { get; }

        public Evaluator ExpressionEvaluator { get; }

        public PluginManager PluginManager { get; }

        public IOutput Out { get; }

        public Dictionary<string, IYamlObject> Parameters { get; } = new Dictionary<string, IYamlObject>();

        public Dictionary<string, IYamlObject> Variables { get; } = new Dictionary<string, IYamlObject>();

        public Dictionary<string, IYamlObject> Providers { get; } = new Dictionary<string, IYamlObject>();

        public Dictionary<string, IYamlObject> Locals { get; } = new Dictionary<string, IYamlObject>();

        public Dictionary<string, ICacActivity> AvailableActivities { get; set; }

        public Dictionary<string, ICacProvider> AvailableProviders { get; set; }

        public List<ICacCommandHandler> AvailableCommandHandlers { get; set; }

        public List<ICacCommand> Commands { get; set; }

        public IYamlConverter YamlConverter { get; set; }

        ReadOnlyDictionary<string, IYamlObject> Cac.IExecutionContext.Parameters => new ReadOnlyDictionary<string, IYamlObject>(Parameters);

        ReadOnlyDictionary<string, IYamlObject> Cac.IExecutionContext.Variables => new ReadOnlyDictionary<string, IYamlObject>(Variables);

        ReadOnlyDictionary<string, IYamlObject> Cac.IExecutionContext.Providers => new ReadOnlyDictionary<string, IYamlObject>(Providers);

    }
}
