using Cac.Extensibility;
using Cac.Output;
using System.Collections.Generic;

namespace Cac.Default.Commands
{
    public class ReplaceCommand : ICacCommand
    {
        public string Sourcefile { get; set; }
        public string Targetfile { get; set; }
        public string Prefix { get; set; }
        public string Sufix { get; set; }
        public Dictionary<string, string> Variables { get; set; }

        public void WritePlan(IOutput output)
        {
            output.WriteLine($"Replace from {Sourcefile}");
            output.WriteLine($"    target {Targetfile}");

            foreach (var kvp in Variables)
            {
                output.WriteLine($"  - {Prefix}{kvp.Key}{Sufix} => {kvp.Value}");
            }
        }
    }
}
