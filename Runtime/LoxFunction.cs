using System.Collections.Generic;
using LoxSharp.Core;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity => Declaration.Parameters.Count;
        public Stmt.Function Declaration { get; }
        public Scope Closure {get;}
        public LoxFunction(Stmt.Function declaration, Scope closure)
        {
            Declaration = declaration;
            Closure = closure;
        }
        public object? Call(Interpreter interpreter, params object?[] arguments) => Call(interpreter, arguments.AsEnumerable());

        public object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
        {
            if (arguments.Count() != Arity)
                throw new ParameterMismatchException($"Expected {Arity} arguments but got {arguments.Count()}");
            Scope functionScope = new Scope(Closure);
            foreach(var (param, arg) in Declaration.Parameters.Zip(arguments))
            {
                functionScope.Define(param, arg);
            }
            try 
            {
                if (Declaration.Body is Stmt.Block block)
                {
                    interpreter.ExecuteBlock(block.Statements, functionScope);
                }
                else
                {
                    interpreter.ExecuteBlock(new List<Stmt> {Declaration.Body}, functionScope);
                }
                return null;
            }
            catch(ReturnException e) 
            {
                return e.Value;
            }
        }
    }
}