using System;
using System.Collections.Generic;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp
{
    public class Lox
    {
        public void Interpret(string path)
        {
            using var sr = SourceReader.FromFile(path);
            var lexer = new Lexer(sr);
            List<Token> tokens = new();
            foreach(var tk in lexer.Lex())
            {
                Console.WriteLine(tk);
                tokens.Add(tk);
            }
            var parser = new Parser(tokens);
            var tree = parser.Parse();
            var interpreter = new Interpreter();
            interpreter.Interpret(tree);
        }
    }
}
