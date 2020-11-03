using System.Collections.Generic;

namespace Cac.Options
{
    public interface ICacOptions
    {
        IEnumerable<string> Parameters { get; }
        bool Verbose { get; }
        string YamlFile { get; }
        string InputFile { get; }
        string OutputFile { get; }
        bool ShouldApply { get; }
    }
}
