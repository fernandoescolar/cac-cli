using Cac.Extensibility;
using Cac.Output;

namespace Cac.Sample
{
    public class SampleCommand : ICacCommand
    {
        public string Name { get; set; }

        public SampleCommand()
        {
        }

        public void WritePlan(IOutput output)
        {
            output.WriteLine($"Name to write: {Name}");
        }
    }
}
