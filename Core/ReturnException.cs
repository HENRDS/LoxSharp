namespace LoxSharp.Core
{
    internal class ReturnException : Exception
    {
        public object? Value {get;}
        public ReturnException(object? value) : base()
        {
            Value = value;
        }

    }
}