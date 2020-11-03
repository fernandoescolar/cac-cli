using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Cac.Options
{
    [Verb("apply", HelpText = "Applies the yaml configurations.")]
    public class ApplyOptions : OptionsBase
    {
        public ApplyOptions()
        {
            ShouldApply = true;
        }

        [Option('i', "input", Required = false, HelpText = "Set input plan file to apply.")]
        public override string InputFile { get; set; }

        [Usage(ApplicationAlias = "cac")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new[] {
                    new Example("Apply", new ApplyOptions { YamlFile = "file.yaml" }),
                    new Example("Apply with input plan file", new ApplyOptions { YamlFile = "file.yaml", InputFile = "plan.json" }),
                    new Example("Apply with parameters", new ApplyOptions { YamlFile = "file.yaml", Parameters = new[]{ "name1=value1", "name2=value2" } }),
                };
            }
        }
    }
}
