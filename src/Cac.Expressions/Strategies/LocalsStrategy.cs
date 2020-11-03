using Cac.Exceptions;
using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;
using System;
using System.Linq;

namespace Cac.Expressions.Strategies
{
    public class LocalsStrategy : IEvalStrategy
    {
        private static readonly ExpressionTokenType[] InvalidTokenTypes = new[] {
            ExpressionTokenType.Integer,
            ExpressionTokenType.StringValue,
            ExpressionTokenType.Float,
            ExpressionTokenType.OpenParenthesis,
            ExpressionTokenType.CloseParenthesis,
            ExpressionTokenType.OpenBracket,
            ExpressionTokenType.CloseBracket,
            ExpressionTokenType.Comma,
            ExpressionTokenType.Dot
        };

        public LocalsStrategy()
        {
        }

        public bool CanEval(Token token)
        {
            return token.Is(ExpressionTokenType.Unknown);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            if (!context.Locals.ContainsKey(token.Value)) throw new UnexpectedCharacterException(token.Line, token.Column, token.Value);
            var result = context.Locals[token.Value];
            do
            {
                var dotToken = context.Stack.NextToken();
                if (!dotToken.Is(ExpressionTokenType.Dot))
                {
                    context.Stack.StepBack();
                    return result;
                }

                var name = context.Stack.NextToken();
                if (InvalidTokenTypes.Contains(token.GetTokenType<ExpressionTokenType>()))
                    throw new UnexpectedTypeException($"Expected string, found `{dotToken.TokenType}`", dotToken.Line, dotToken.Column, dotToken.Value);

                if (result is YamlMappingObject m)
                {
                    if (!m.Children.ContainsKey(name.Value))
                        throw new PropertyNotFoundException(name.Value, m.Line, m.Column, default);

                    result = m.Children[name.Value];
                }
                else
                {
                    throw new UnexpectedTypeException($"Expected `YamlMappingObject` found `{result.GetType().Name}`", result.Line, result.Column);
                }
            } while (true);
        }
    }
}
