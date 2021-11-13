namespace LoxSharp.Core
{
    internal class ParameterMismatchException : Exception
    {
        public ParameterMismatchException(string? message) : base(message)
        {
        }

        public ParameterMismatchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}