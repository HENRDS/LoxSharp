using System;

namespace LoxSharp
{
    class Program
    {
        static void ParseFile()
        {
            var lox = new Lox();
            lox.Interpret("Tests/simple.lox");
        }
        static void Main(string[] args)
        {
            ParseFile();
        }
    }
}
