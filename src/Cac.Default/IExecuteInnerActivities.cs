using Cac.Extensibility;
using Cac.Yaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Default
{
    public interface IExecuteInnerActivities
    {
        Action<IYamlObject> EvaluateExpressions { get; set; }

        Func<IYamlObject, Task<IEnumerable<ICacCommand>>> PlanActivity { get; set; }
    }
}
