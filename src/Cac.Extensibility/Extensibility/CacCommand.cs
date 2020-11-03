using Cac.Output;

namespace Cac.Extensibility
{
    public abstract class CacCommand : ICacCommand
    {
        public void WritePlan(IOutput output)
        {
            OnWritePlan(output);
        }

        protected abstract void OnWritePlan(IOutput output);
    }
}
