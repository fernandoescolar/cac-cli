namespace Cac.Output
{
    public interface IOutput : IOutputWriter
    {
        IOutputWriter Verbose { get; }

        public void BeginSection(string title = default);

        public void EndSection();

        public void ResetSections();
    }
}
