using Cac.Extensibility;
using Cac.Options;
using Cac.Output;
using Cac.Yaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cac.Expressions
{
    public class EvalContext : IEvalContext
    {
        private readonly Evaluator _evaluator;
        private readonly IExecutionContext _innerContext;

        public EvalContext(Evaluator evaluator, IExecutionContext innerContext)
        {
            _evaluator = evaluator;
            _innerContext = innerContext;
        }

        public TokenStack Stack => _evaluator.Stack;

        public ICacOptions Options => _innerContext.Options;
        
        public IOutput Out => _innerContext.Out;

        public ReadOnlyDictionary<string, IYamlObject> Parameters => _innerContext.Parameters;

        public ReadOnlyDictionary<string, IYamlObject> Variables => _innerContext.Variables;

        public ReadOnlyDictionary<string, IYamlObject> Providers => _innerContext.Providers;

        public List<ICacCommand> Commands => _innerContext.Commands;

        public Dictionary<string, IYamlObject> Locals => _innerContext.Locals;

        public IYamlObject EvalNextObject() => _evaluator.EvalNextObject();
    }
}
