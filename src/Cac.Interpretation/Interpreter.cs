using Cac.Interpretation.NodeProcessors;
using Cac.Interpretation.Strategies;
using Cac.Options;
using Cac.Output;
using Cac.Yaml;
using Cac.Yaml.Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cac.Interpretation
{
    public class Interpreter
    {
        private readonly IExecutionContext _context;
        private readonly IEnumerable<INodeProcessor> _processors;
        private readonly IEnumerable<IInterpreterStrategy> _strategies;

        public Interpreter(ICacOptions options, IOutput output)
        {
            _context = new ExecutionContext(options, output);
            _processors = new INodeProcessor[] // the order is important
            {
                new ParametersNodeProcessor(),
                new ParametersFromEnvironmentNodeProcessor(),
                new ParametersFromOptionsNodeProcessor(),
                new PackagesNodeProcessor(),
                new VariablesNodeProcessor(),
                new ProvidersNodeProcessor(),
                new ActivitiesNodeProcessor()
            };
            _strategies = new IInterpreterStrategy[] {
                new InputFilePlanStrategy(),
                new PlanStrategy(),
                new ApplyStrategy()
            };
        }

        public void Process(string filepath)
        {
            ProcessYaml(filepath);
            ExecuteStrategies();
        }

        private void ProcessYaml(string filepath)
        {
            var document = File.ReadAllText(filepath);
            var yaml = new YamlParser();
            var root = yaml.Parse(document) as YamlMappingObject;
            foreach (var entry in root.Children)
            {
                ProcessEntry(entry.Key, entry.Value);
            }
        }

        private void ProcessEntry(string name, IYamlObject value)
        {
            name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException(nameof(name));
            value = value ?? throw new ArgumentException(nameof(value));
            _context.Out.BeginSection(name);
            foreach (var processor in _processors)
            {
                if (processor.CanProcess(name))
                {
                    _context.Out.Verbose.WriteLine($"Processing with {processor.GetType().Name}", ConsoleColor.Cyan);
                    processor.Process(value, _context);
                }
            }

            _context.Out.EndSection();
        }

        private void ExecuteStrategies()
        {
            foreach (var strategy in _strategies)
            {
                strategy.Execute(_context);
            }
        }
    }
}
