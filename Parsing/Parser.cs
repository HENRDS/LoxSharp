using System.Collections.Generic;
using LoxSharp.Lexing;
using System.Linq;

namespace LoxSharp.Parsing
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
        }
        private Token Peek(int offset = 0) => tokens[current + offset];
        private bool IsAtEnd => Peek().Type == TokenType.Eof;

        private Token Advance()
        {
            if (!IsAtEnd)
                current++;
            return Peek(-1);
        }

        private bool Check(params TokenType[] types) => types.Contains(Peek().Type);
        private bool Match(params TokenType[] types)
        {
            if (types.Contains(tokens[current].Type))
            {
                Advance();
                return true;
            }
            return false;
        }
        private Expr Expression()
        {

        }

    }
}
