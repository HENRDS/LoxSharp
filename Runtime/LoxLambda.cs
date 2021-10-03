using System.Collections.Generic;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class LoxLambda : ILoxCallable
    {
        public int Arity { get; }
        
        public LoxLambda(List<Token> Parameters, Expr body)
        {
            
        }
        public object? Call(Interpreter evaluator, params object[] arguments)
        {
            throw new System.NotImplementedException();
        }
    }
}