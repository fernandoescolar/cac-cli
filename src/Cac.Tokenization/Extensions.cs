using System;
using System.Linq;

namespace Cac.Tokenization
{
    public static class Extensions
    {
        public static T GetTokenType<T>(this Token token)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("The generic parameter kind should be `Enum`", nameof(T));

            return (T)Enum.Parse(typeof(T), token.TokenType);
        }

        public static bool Is<T>(this Token token, T type)
        {
            return token != null && GetTokenType<T>(token).Equals(type);
        }

        public static bool IsIn<T>(this Token token, params T[] types)
        {
            return token != null && types.Contains(GetTokenType<T>(token));
        }
    }
}

