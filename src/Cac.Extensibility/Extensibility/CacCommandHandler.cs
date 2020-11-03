using System;
using System.Threading.Tasks;

namespace Cac.Extensibility
{
    public abstract class CacCommandHandler<T> : ICacCommandHandler<T>
        where T : ICacCommand
    {
        public bool CanHandle(object command)
        {
            return command is T;
        }

        public Task HandleAsync(T command, IExecutionContext context)
        {
            return OnHandleAsync(command, context);
        }

        public Task HandleAsync(object command, IExecutionContext context)
        {
            if (command is T typedCommand)
            {
                return HandleAsync(typedCommand, context);
            }

            throw new ArgumentException(nameof(command), "Unspected command type");
        }

        protected abstract Task OnHandleAsync(T command, IExecutionContext context);
    }
}
