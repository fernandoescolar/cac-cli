using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public interface ICacCommandHandler
    {
        bool CanHandle(object command);
        Task HandleAsync(object command, IExecutionContext context);
    }

    public interface ICacCommandHandler<in T> : ICacCommandHandler
        where T : ICacCommand
    {
        Task HandleAsync(T command, IExecutionContext context);
    }
}
