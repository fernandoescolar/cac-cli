using Cac.Yaml;
using System;

namespace Cac.Interpretation.NodeProcessors
{
    public static class ExecutionContextExtensions
    {
        public static void VerboseWriteAdding(this IExecutionContext context, string parameterName, IYamlObject yaml)
        {
            context.Out.Verbose.Write($"Adding", ConsoleColor.Cyan);
            context.Out.Verbose.Write($": ");
            context.Out.Verbose.Write(parameterName, ConsoleColor.Magenta);
            context.Out.Verbose.Write(" => ");
            if (yaml.ToString().Contains("\n"))
            {
                context.Out.Verbose.WriteLine(string.Empty);
                context.Out.BeginSection(string.Empty);
                context.Out.Verbose.WriteLine(yaml.ToString(), ConsoleColor.White);
                context.Out.EndSection();
            }
            else
            {
                context.Out.Verbose.WriteLine(yaml.ToString(), ConsoleColor.White);
            }
        }
    }
}
