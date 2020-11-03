using Cac.Extensibility;
using Cac.Options;
using Cac.Output;
using Cac.Yaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cac
{
    public interface IExecutionContext
    {
        ReadOnlyDictionary<string, IYamlObject> Parameters { get; }

        ReadOnlyDictionary<string, IYamlObject> Variables { get; }

        ReadOnlyDictionary<string, IYamlObject> Providers { get; }

        Dictionary<string, IYamlObject> Locals { get; }

        List<ICacCommand> Commands { get; }

        ICacOptions Options { get; }

        IOutput Out { get; }
    }
} 
