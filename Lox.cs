using System;
using LoxSharp.Lexing;
namespace LoxSharp
{
    public class Lox
    {
        public void Interpret(string path)
        {
            using var sr = SourceReader.FromFile(path);
            var lexer = new Lexer(sr);
            foreach(var tk in lexer.Lex())
                Console.WriteLine(tk);
        }
    }
}
