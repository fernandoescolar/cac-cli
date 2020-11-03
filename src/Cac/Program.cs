using Cac.ErrorHandling;
using Cac.Interpretation;
using Cac.Options;
using Cac.Output;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace Cac
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.HelpWriter = null;
                with.CaseSensitive = false;
                with.EnableDashDash = true;
            });
            var parserResult = parser.ParseArguments<PlanOptions, ApplyOptions>(args);
            parserResult
                  .WithParsed<PlanOptions>(options => Run(options))
                  .WithParsed<ApplyOptions>(options => Run(options))
                  .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            Console.WriteLine(helpText);
        }

        private static void Run(PlanOptions options)
        {
            RunCommand(options);
        }

        private static void Run(ApplyOptions options)
        {
            RunCommand(options);
        }

        private static void RunCommand(OptionsBase options)
        {
            var output = new TerminalOutput(options);
            var interpreter = new Interpreter(options, output);
            try
            {
                interpreter.Process(options.YamlFile);
                Environment.ExitCode = (int)ExitCodes.Success;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, output, options.YamlFile);
                Environment.ExitCode = (int)ExitCodes.Error;
            }
        }
    }
}
