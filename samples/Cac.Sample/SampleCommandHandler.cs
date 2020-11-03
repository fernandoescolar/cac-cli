using System.Threading.Tasks;
using Cac.Extensibility;

namespace Cac.Sample
{
    public class SampleCommandHandler : CacCommandHandler<SampleCommand>
    {
        protected override Task OnHandleAsync(SampleCommand command, IExecutionContext context)
        {
            context.Out.WriteLine($"hello: {command.Name}");
            return Task.CompletedTask;
        }
    }
}
