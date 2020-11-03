using Cac.Expressions;
using Cac.Extensibility;
using Cac.Plugins;
using Cac.Yaml;
using System.Collections.Generic;

namespace Cac.Interpretation
{
    public interface IExecutionContext : Cac.IExecutionContext
    {
        PluginManager PluginManager { get; }

        Evaluator ExpressionEvaluator { get; }

        new Dictionary<string, IYamlObject> Parameters { get; }

        new Dictionary<string, IYamlObject> Variables { get; }

        new Dictionary<string, IYamlObject> Providers { get; }

        Dictionary<string, ICacActivity> AvailableActivities { get; set; }

        Dictionary<string, ICacProvider> AvailableProviders { get; set; }

        List<ICacCommandHandler> AvailableCommandHandlers { get; set; }

        IYamlConverter YamlConverter { get; set; }
    }
}
