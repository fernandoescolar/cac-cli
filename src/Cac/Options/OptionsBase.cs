using CommandLine;
using System.Collections.Generic;

namespace Cac.Options
{
    public abstract class OptionsBase : ICacOptions
    {
        [Value(0, MetaName = "input file", HelpText = "Yaml input file to be processed.", Required = true)]
        public string YamlFile { get; set; }

        [Option('p', "parameters", Required = false, HelpText = "Yaml input parameters. You can use this format: name1=value1;name2=value2", Min = 1, Separator = ';')]
        public IEnumerable<string> Parameters { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        public bool ShouldApply { get; protected set; }

        public virtual string InputFile { get; set; }

        public virtual string OutputFile { get; set; }
    }
}
