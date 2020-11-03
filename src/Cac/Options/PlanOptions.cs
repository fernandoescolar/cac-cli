using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Cac.Options
{
    [Verb("plan", HelpText = "Shows the work plan for a yaml file.")]
    public class PlanOptions : OptionsBase
    {
        [Option('o', "output", Required = false, HelpText = "Set plan output file.")]
        public override string OutputFile { get; set; }

        [Usage(ApplicationAlias = "cac")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new[] {
                    new Example("Plan", new PlanOptions { YamlFile = "file.yaml" }),
                    new Example("Plan with output file", new PlanOptions { YamlFile = "file.yaml", OutputFile = "plan.json" }),
                    new Example("Plan with parameters", new PlanOptions { YamlFile = "file.yaml", Parameters = new[]{ "name1=value1", "name2=value2" } }),
                };
            }
        }
    }
}
