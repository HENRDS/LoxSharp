namespace LoxSharp.Core
{
    public class ParseError : System.Exception
    {
        public ParseError() {}
        public ParseError(string message) : base(message) {}
        public ParseError(string message, System.Exception inner) : base(message, inner) {}
        public ParseError(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}
