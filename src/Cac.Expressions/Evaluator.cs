using Cac.Exceptions;
using Cac.Expressions.Parser;
using Cac.Tokenization;
using Cac.Yaml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cac.Expressions
{
    public class Evaluator
    {
        private readonly IEvalContext _evalContext;
        private readonly ExpressionsTokenizer _tokenizer;
        private readonly IEnumerable<IEvalStrategy> _strategies;

        public Evaluator(IExecutionContext context)
        {
            _evalContext = new EvalContext(this, context);
            _tokenizer = new ExpressionsTokenizer();
            _strategies = typeof(Evaluator).Assembly
                                           .GetTypes()
                                           .Where(x => !x.IsAbstract && typeof(IEvalStrategy).IsAssignableFrom(x))
                                           .Select(x => (IEvalStrategy)Activator.CreateInstance(x))
                                           .ToList();
        }

        public TokenStack Stack { get; private set; }

        public IYamlObject Eval(string content)
        {
            var tokens = _tokenizer.Tokenize(content).ToList();

            Stack = new TokenStack(tokens);
            var result = EvalNextObject();

            if (!Stack.HasEnded)
            {
                ThrowUnexpectedTokenExpection(Stack.NextToken());
            }

            _evalContext.Out.Verbose.Write($"eval", ConsoleColor.Cyan);
            _evalContext.Out.Verbose.Write($": ");
            _evalContext.Out.Verbose.Write(content, ConsoleColor.DarkYellow);
            _evalContext.Out.Verbose.Write(" => ");
            if (result.ToString().Contains("\n"))
            {
                _evalContext.Out.Verbose.WriteLine(string.Empty);
                _evalContext.Out.BeginSection(string.Empty);
                _evalContext.Out.Verbose.WriteLine(result.ToString(), ConsoleColor.White);
                _evalContext.Out.EndSection();
            }
            else
            {
                _evalContext.Out.Verbose.WriteLine(result.ToString(), ConsoleColor.White);
            }
            
            return result;
        }

        internal IYamlObject EvalNextObject()
        {
            var token = Stack.NextToken();
            foreach (var strategy in _strategies)
            {
                if (strategy.CanEval(token))
                {
                    return strategy.Eval(_evalContext, token);
                }
            }

            ThrowUnexpectedTokenExpection(token);
            return default;
        }

        private void ThrowUnexpectedTokenExpection(Token token)
        {
            throw new UnexpectedCharacterException(token.Line, token.Column, token.Value);
        }
    }
}
