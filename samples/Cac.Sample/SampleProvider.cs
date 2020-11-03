using Cac.Extensibility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Sample
{
    [Name("sample")]
    public class SampleProvider : CacProvider<Dictionary<string, object>, Dictionary<string, object>>
    {
        protected override Task<Dictionary<string, object>> OnGetValueAsync(Dictionary<string, object> model, IExecutionContext context)
        {
            // removes entry name
            if (model.ContainsKey("sample"))  model.Remove("sample");

            return Task.FromResult(model);
        }
    }
}
