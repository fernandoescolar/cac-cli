using System;

namespace Cac.Yaml
{
    public interface IYamlObject : ICloneable
    {
        bool IsEmpty { get; }

        int Line { get; set; }

        int Column { get; set; }

        string ToString(int spaces, bool isSequence, bool isMapping);
    }
}
