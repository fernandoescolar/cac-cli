using Cac.Exceptions;
using Cac.Extensibility;
using Cac.Yaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cac.Default.Activities
{
    [Name("for_each")]
    public class ForEachActivity : ICacActivity, IExecuteInnerActivities
    {
        public Action<IYamlObject> EvaluateExpressions { get; set; }

        public Func<IYamlObject, Task<IEnumerable<ICacCommand>>> PlanActivity { get; set; }

        public async Task<IEnumerable<ICacCommand>> PlanAsync(IYamlObject entry, IYamlConverter yamlConverter, IExecutionContext context)
        {
            if (entry is not YamlMappingObject node) throw new UnexpectedTypeException("Yaml node should be a mapping", entry.Line, entry.Column);
            if (!node.Children.ContainsKey("for_each")) throw new PropertyNotFoundException("for_each", node.Line, node.Column);
            if (!node.Children.ContainsKey("as")) throw new PropertyNotFoundException("as", node.Line, node.Column);

            var nodeToEval = new YamlMappingObject
            {
                Line = node.Line,
                Column = node.Column
            };

            nodeToEval.ChildrenAdd("for_each", node.Children["for_each"]);
            nodeToEval.ChildrenAdd("as", node.Children["as"]);
            EvaluateExpressions(nodeToEval);

            var paramName = (nodeToEval.Children["as"] as YamlScalarObject)?.Value;

            if (nodeToEval.Children["for_each"] is not YamlSequenceObject values) throw new UnexpectedYamlObjectException("It has not a sequence", node);
            if (string.IsNullOrEmpty(paramName)) throw new UnexpectedYamlObjectException("It has not a sequence variable name", node);
            if (node.Children["activities"] is not YamlSequenceObject activities) throw new UnexpectedYamlObjectException("It has not sub activities", node);

            context.Out.BeginSection("for_each");
            var result = new List<ICacCommand>();
            foreach (var value in values)
            {
                var clonedActivities = (YamlSequenceObject)activities.Clone();
                context.Locals.Add(paramName, value);
                context.Out.Verbose.Write(paramName, ConsoleColor.Yellow);
                context.Out.Verbose.Write(" = ");
                if (value.ToString().Contains("\n"))
                {
                    context.Out.Verbose.WriteLine(string.Empty);
                    context.Out.BeginSection(string.Empty);
                    context.Out.Verbose.WriteLine(value.ToString(), ConsoleColor.White);
                    context.Out.EndSection();
                }
                else
                {
                    context.Out.Verbose.WriteLine(value.ToString(), ConsoleColor.White);
                }

                foreach (var activity in clonedActivities)
                {
                    context.Out.BeginSection("activities");
                    EvaluateExpressions(activity);
                    result.AddRange(await PlanActivity(activity));
                    context.Out.EndSection();
                }

                context.Locals.Clear();
            }
            context.Out.EndSection();

            return result;
        }
    }
}
