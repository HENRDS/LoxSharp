using LoxSharp.Lexing;

namespace LoxSharp
{
    class Program
    {
        static void ParseFile()
        {
            var lox = new Lox();
            if ('\0'.IsIdentifierStartChar())
                Console.WriteLine("AAAA");
            lox.Interpret("Tests/simple.lox");
        }
        static void Main(string[] args)
        {
            ParseFile();
        }
    }
}
