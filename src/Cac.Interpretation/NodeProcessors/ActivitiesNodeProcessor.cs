using Cac.Default;
using Cac.Exceptions;
using Cac.Expressions;
using Cac.Extensibility;
using Cac.Output;
using Cac.Yaml;
using Cac.Yaml.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cac.Interpretation.NodeProcessors
{
    public class ActivitiesNodeProcessor : INodeProcessor
    {
        public bool CanProcess(string key)
        {
            return "activities".Equals(key, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Process(IYamlObject node, IExecutionContext context)
        {
            if (!ShouldProcess(context)) return;

            var sequence = node as YamlSequenceObject ?? throw new UnexpectedTypeException("Yaml node should be a sequence", node.Line, node.Column);
            var tasks = new List<Task<IEnumerable<ICacCommand>>>();
            foreach (var entry in sequence)
            {
                var task = PlanActivityAsync(entry, context);
                if (IsSyncActivity(entry))
                {
                    task.Wait();
                }
                else
                {
                    tasks.Add(task);
                }
            }

            Task.WaitAll(tasks.ToArray());
            context.Commands.AddRange(tasks.SelectMany(x => x.Result));
        }

        private bool ShouldProcess(IExecutionContext context)
        {
            var isApplyWithInputFile = context.Options.ShouldApply && !string.IsNullOrWhiteSpace(context.Options.InputFile);
            return !isApplyWithInputFile;
        }

        private Task<IEnumerable<ICacCommand>> PlanActivityAsync(IYamlObject entry, IExecutionContext context)
        {
            if (entry is not YamlMappingObject m) throw new UnexpectedTypeException("Yaml node should be a mapping", entry.Line, entry.Column);

            var activityName = m.Children.First().Key;
            var key = context.AvailableActivities.Keys.FirstOrDefault(x => x.Equals(activityName, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(key))
            {
                OutputNotFound(context.Out, activityName);
                return EmptyResult();
            }

            var activity = context.AvailableActivities[key];
            if (activity is IExecuteInnerActivities a)
            {
                a.EvaluateExpressions = yaml => context.ExpressionEvaluator.EvaluateExpressions(yaml);
                a.PlanActivity = yaml => PlanActivityAsync(yaml, context);
            }
            else
            {
                context.ExpressionEvaluator.EvaluateExpressions(m);
            }

            var verbose = !m.Children.ContainsKey("display_name");
            var displayName = !verbose ? m.MapProperty<string>("display_name") : activityName;
            if (m.Children.ContainsKey("if"))
            {
                var condition = m.Children["if"].Map<bool>();
                if (!condition)
                {
                    OutputSkip(context.Out, displayName);
                    return EmptyResult();
                }
            }

            OutputPlanning(context.Out, verbose, displayName);
            return activity.PlanAsync(m, context.YamlConverter, context);
        }

        private static Task<IEnumerable<ICacCommand>> EmptyResult() => Task.FromResult<IEnumerable<ICacCommand>>(new ICacCommand[0]);

        private static void OutputNotFound(IOutput output, string activityName)
        {
            output.Error($"Error: Activity '{activityName}' not found.");
        }

        private static void OutputPlanning(IOutput output, bool verbose, string displayName)
        {
            if (!verbose)
            {
                output.WriteLine(displayName, ConsoleColor.Green);
            }
            else
            {
                output.Verbose.WriteLine(displayName, ConsoleColor.Green);
            }
        }

        private static void OutputSkip(IOutput output, string displayName)
        {
            output.Verbose.Write("skip", ConsoleColor.DarkYellow);
            output.Verbose.Write(" activity `");
            output.Verbose.Write(displayName, ConsoleColor.Green);
            output.Verbose.WriteLine("`");
        }

        private static bool IsSyncActivity(IYamlObject entry) => entry is YamlMappingObject m && m.Children.ContainsKey("sync") && m.MapProperty<bool>("sync");
    }
}
