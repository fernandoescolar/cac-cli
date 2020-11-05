using Cac.Extensibility;
using Cac.Yaml;
using System;
using System.Threading.Tasks;

namespace Cac.Default.Providers
{
    [Name("random")]
    public class RandomProvider : CacProvider<RandomResult>
    {
        protected override Task<RandomResult> OnGetValueAsync(IExecutionContext context)
        {
            var rnd = new Random();
            var result = new RandomResult
            {
                Double = rnd.NextDouble(),
                Integer = rnd.Next(),
                Boolean = rnd.Next(1, 100) > 50
            };

            return Task.FromResult(result);
        }
    }

    public class RandomResult
    {
        [YamlProperty("double")]
        public double Double { get; set; }

        [YamlProperty("integer")]
        public int Integer { get; set; }

        [YamlProperty("boolean")]
        public bool Boolean { get; set; }
    }
}
