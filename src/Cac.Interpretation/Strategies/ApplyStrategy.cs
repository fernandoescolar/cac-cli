using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Interpretation.Strategies
{
    public class ApplyStrategy : IInterpreterStrategy
    {
        public void Execute(IExecutionContext context)
        {
            if (!context.Options.ShouldApply)
                return;

            var tasks = new List<Task>();
            context.Out.BeginSection("apply");
            foreach (var command in context.Commands)
            {
                foreach (var handler in context.AvailableCommandHandlers)
                {
                    if (handler.CanHandle(command))
                    {
                        context.Out.Verbose.Write("executing command handler `");
                        context.Out.Verbose.Write(command.GetType().FullName, ConsoleColor.Green);
                        context.Out.Verbose.WriteLine("`");
                        tasks.Add(handler.HandleAsync(command, context));
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());
            context.Out.EndSection();
        }
    }
}
