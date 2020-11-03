namespace Cac.Yaml
{
    public interface IYamlConverter
    {
        T To<T>(IYamlObject yaml);

        IYamlObject From(object o);
    }
}
