using Cac.Extensibility;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Cac.Interpretation.Strategies
{
    public class InputFilePlanStrategy : IInterpreterStrategy
    {
        public void Execute(IExecutionContext context)
        {
            if (context.Options.ShouldApply && !string.IsNullOrWhiteSpace(context.Options.InputFile))
            {
                context.Out.BeginSection("plan");
                context.Out.WriteLine($"file: {context.Options.InputFile}");
                context.Out.EndSection();

                if (!File.Exists(context.Options.InputFile))
                    throw new FileNotFoundException($"Can not find input plan file: {context.Options.InputFile}");

                var json = File.ReadAllText(context.Options.InputFile);
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                };
                var commands = JsonConvert.DeserializeObject<Collection<ICacCommand>>(json, settings);
                context.Commands.Clear();
                context.Commands.AddRange(commands);
            }
        }
    }
}
