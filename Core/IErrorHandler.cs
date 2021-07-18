using LoxSharp.Lexing;

namespace LoxSharp.Core
{
    public interface IErrorHandler
    {
        public bool HadError { get; }
        public void Error(Token token, string message);
    }
}
