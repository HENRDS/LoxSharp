using System.Collections.Generic;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class LoxLambda : ILoxCallable
    {
        public int Arity { get; }
        public List<Token> Parameters { get; }
        public Expr Body { get; }

        public LoxLambda(List<Token> parameters, Expr body)
        {
            Parameters = parameters;
            Body = body;
        }
        public object? Call(Interpreter interpreter, params object?[] arguments) => 
            Call(interpreter, arguments.AsEnumerable());

        public object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
        {
            throw new NotImplementedException();
        }
    }
}