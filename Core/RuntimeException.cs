using LoxSharp.Lexing;

namespace LoxSharp.Core
{
    [System.Serializable]
    public class RuntimeException : System.Exception
    {
        public Token Token { get; set; }
        public RuntimeException(Token token) { 
            Token = token;
        }
        public RuntimeException(Token token, string message) : base(message) { 
            Token = token;
        }
        public RuntimeException(Token token, string message, System.Exception inner) : base(message, inner) {
            Token = token;
         }
    }
}