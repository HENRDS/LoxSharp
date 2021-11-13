namespace LoxSharp.Runtime
{
    public class LoxNative : ILoxCallable
    {

        public int Arity {get;}
        private Func<List<object?>, object?> func;
        public LoxNative(int arity, Func<List<object?>, object?> func)
        {
            Arity = arity;
            this.func = func;
        }
        public object? Call(Interpreter interpreter, params object?[] arguments) => Call(interpreter, arguments.AsEnumerable());

        public object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
        {
            return func(arguments.ToList());
        }
    }
}