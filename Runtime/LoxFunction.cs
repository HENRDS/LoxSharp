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
        public Scope Closure { get; }
        public bool IsInitializer { get; }
        public LoxFunction(Stmt.Function declaration, Scope closure, bool isInitializer)
        {
            Declaration = declaration;
            Closure = closure;
            IsInitializer = isInitializer;
        }
        public LoxFunction Bind(LoxInstance instance)
        {
            Scope scope = new(Closure);
            scope.SafeDefine("this", instance);
            return new LoxFunction(Declaration, scope, IsInitializer);
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
                if (IsInitializer)
                    return Closure.GetAt("this", 0);
                return null;
            }
            catch(ReturnException e) 
            {
                if (IsInitializer)
                    return Closure.GetAt("this", 0);
                return e.Value;
            }
        }
    }
}