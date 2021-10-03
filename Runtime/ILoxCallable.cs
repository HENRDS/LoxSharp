using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public interface ILoxCallable
    {
        int Arity { get; }
        object? Call(Interpreter evaluator, params object[] arguments);
    }    
}