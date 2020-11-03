using Cac.Extensibility;
using Cac.Output;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cac.Interpretation.Strategies
{
    public class PlanStrategy : IInterpreterStrategy
    {
        public void Execute(IExecutionContext context)
        {
            if (context.Options.ShouldApply 
                && !string.IsNullOrWhiteSpace(context.Options.OutputFile)
                && !context.Options.Verbose)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(context.Options.OutputFile))
            {
                WriteOutputFile(context.Commands, context.Options.OutputFile);
            }

            WriteOutput(context.Commands, context.Out);
        }

        private void WriteOutput(IEnumerable<ICacCommand> commands, IOutput output)
        {
            output.BeginSection("plan");
            if (commands.Count() == 0)
            {
                output.WriteLine("nothing planned", System.ConsoleColor.DarkYellow);
            }

            foreach (var command in commands)
            {
                command.WritePlan(output);
            }

            output.EndSection();
        }

        private void WriteOutputFile(IEnumerable<ICacCommand> commands, string outputFile)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            var json = JsonConvert.SerializeObject(commands, settings);
            File.WriteAllText(outputFile, json);
        }
    }
}
