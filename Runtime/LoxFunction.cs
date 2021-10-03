using System.Collections.Generic;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity { get; }
        public LoxFunction(List<Token> @params)
        {
            
        }
        public object? Call(Interpreter evaluator, params object[] arguments)
        {
            throw new System.NotImplementedException();
        }
    }
}