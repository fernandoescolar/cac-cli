using Cac.Extensibility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Sample
{
    [Name("sample")]
    public class SampleActivity : CacActivity<SampleModel>
    {
        protected override Task<IEnumerable<ICacCommand>> OnPlanAsync(SampleModel model, IExecutionContext context)
        {
            var command = new SampleCommand { Name = model.Name };
            return Task.FromResult<IEnumerable<ICacCommand>>(new ICacCommand[] { command });
        }
    }
}
