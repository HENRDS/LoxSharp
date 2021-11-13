using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public interface ILoxCallable
    {
        int Arity { get; }
        object? Call(Interpreter interpreter, params object?[] arguments);
        object? Call(Interpreter interpreter, IEnumerable<object?> arguments);
    }    
}