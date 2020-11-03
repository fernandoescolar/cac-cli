using Cac.Yaml;

namespace Cac.Sample
{
    public class SampleModel
    {
        [YamlProperty("sample")]
        public string Name { get; set; }
    }
}
