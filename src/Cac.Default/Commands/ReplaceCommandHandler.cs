using Cac.Extensibility;
using System.IO;
using System.Threading.Tasks;

namespace Cac.Default.Commands
{
    public class ReplaceCommandHandler : CacCommandHandler<ReplaceCommand>
    {
        protected override Task OnHandleAsync(ReplaceCommand command, IExecutionContext context)
        {
            if (!File.Exists(command.Sourcefile))
                throw new FileNotFoundException("File not found", command.Sourcefile);

            var content = File.ReadAllText(command.Sourcefile);
            foreach (var v in command.Variables)
            {
                var key = $"{command.Prefix}{v.Key}{command.Sufix}";
                content = content.Replace(key, v.Value);
            }

            File.WriteAllText(command.Targetfile, content);
            return Task.CompletedTask;
        }
    }
}
