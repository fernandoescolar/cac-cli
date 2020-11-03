using Cac.Tokenization;

namespace Cac.Yaml.Parser
{
    public static class Extensions
    {
        public static YamlTokenType GetYamlTokenType(this Token token)
        {
            return token.GetTokenType<YamlTokenType>();
        }
    }
}
