using Cac.Exceptions;
using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cac.Expressions.Strategies
{
    public abstract class ObjectPropertyStrategy : IEvalStrategy
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
        private readonly ExpressionTokenType _type;

        protected ObjectPropertyStrategy(ExpressionTokenType type)
        {
            _type = type;
        }

        public bool CanEval(Token token)
        {
            return token.Is(_type);
        }

        public IYamlObject Eval(IEvalContext context, Token token)
        {
            var first = true;
            var result = default(IYamlObject);
            do
            {
                var dotToken = context.Stack.NextToken();
                if (!dotToken.Is(ExpressionTokenType.Dot))
                {
                    if (first) throw new UnexpectedTypeException($"Expected dot (.) token type, found `{dotToken.TokenType}`", dotToken.Line, dotToken.Column, dotToken.Value);
                    else
                    {
                        context.Stack.StepBack();
                        return result;
                    }
                }

                var name = context.Stack.NextToken();
                if (InvalidTokenTypes.Contains(token.GetTokenType<ExpressionTokenType>()))
                    throw new UnexpectedTypeException($"Expected string, found `{dotToken.TokenType}`", dotToken.Line, dotToken.Column, dotToken.Value);

                if (first)
                {
                    result = GetObject(context)[name.Value];
                    first = false;
                }
                else
                {
                    if (result is YamlMappingObject m)
                    {
                        result = m.Children[name.Value];
                    }
                    else
                    {
                        throw new UnexpectedTypeException($"Expected `YamlMappingObject`, found `{result.GetType().Name}`", result.Line, result.Column);
                    }
                }
            } while (true);
        }

        protected abstract ReadOnlyDictionary<string, IYamlObject> GetObject(IEvalContext context);
    }
}
