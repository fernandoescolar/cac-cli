using Cac.Output;

namespace Cac.Extensibility
{
    public interface ICacCommand
    {
        void WritePlan(IOutput output);
    }
}
