using System;
using System.Collections.Generic;

namespace LoxSharp.Lexing
{
    public class Lexer
    {
        public static readonly HashSet<string> Keywords = new() {
            "and", "break", "const", "class", "continue", "else", "enum",
            "for", "fun", "if", "in", "not", "or", "return", "while" };
        private readonly SourceReader reader;
        public Lexer(SourceReader reader)
        {
            this.reader = reader;
        }
        private Token EmitToken(TokenType type, object? value = null)
        {
            var token = new Token(type, reader.Position, reader.Lexeme, value);

            reader.Sync();
            return token;
        }
        private Token NumberLiteral()
        {
            while (reader.Current.IsDigit())
                reader.Advance();
            if (reader.Match('.'))
            {
                while (reader.Current.IsDigit())
                    reader.Advance();
            }
            if (double.TryParse(reader.Lexeme, out double literal))
                return EmitToken(TokenType.NumberLit, literal);
            return EmitToken(TokenType.Invalid);
        }
        private Token StringLiteral()
        {
            while(reader.Peek() != '"')
            {
                if (reader.Peek() == '\n')
                {
                    return EmitToken(TokenType.Invalid);
                }
                reader.Advance();
            }
            reader.Advance(2);
            return EmitToken(TokenType.StringLit, reader.Lexeme.Trim('"'));
        }
        private Token Identifier()
        {
            while (reader.Peek().IsIdentifierChar())
                reader.Advance();
            reader.Advance();
            if (Keywords.Contains(reader.Lexeme))
                return EmitToken(Enum.Parse<TokenType>(reader.Lexeme, true));
            return EmitToken(TokenType.Identifier);
        }
        private Token NextToken()
        {
            char c = reader.Current;
            reader.Advance();
            switch (c)
            {
                case '(': return EmitToken(TokenType.LeftParen);
                case ')': return EmitToken(TokenType.RightParen);
                case '[': return EmitToken(TokenType.LeftBracket);
                case ']': return EmitToken(TokenType.RightBracket);
                case '{': return EmitToken(TokenType.LeftBrace);
                case '}': return EmitToken(TokenType.RightBrace);
                case '+': return EmitToken(TokenType.Plus);
                case '-': return EmitToken(TokenType.Minus);
                case '*': return EmitToken(TokenType.Star);
                case '/': return EmitToken(TokenType.Slash);
                case '|': return EmitToken(TokenType.Pipe);
                case '&': return EmitToken(TokenType.Ampersand);
                case '~': return EmitToken(TokenType.Tilde);
                case ',': return EmitToken(TokenType.Comma);
                case ':': return EmitToken(TokenType.Colon);
                case '.': return EmitToken(TokenType.Dot);
                case ';': return EmitToken(TokenType.Semicolon);
                case ' ':
                    while (reader.Peek() == ' ')
                        reader.Advance();
                    return EmitToken(TokenType.Whitespace);

                case '\n':
                    reader.HandleNewLine();
                    return EmitToken(TokenType.NewLine);
                case '=': return EmitToken(reader.Match('=') ? TokenType.Equal2 : TokenType.Equal);
                case '!':
                    if (reader.Match('='))
                        return EmitToken(TokenType.BangEqual);
                    return EmitToken(TokenType.Invalid);
                case '"':
                    return StringLiteral();
                default:
                    if (c.IsDigit())
                        return NumberLiteral();
                    else if (c.IsIdentifierStartChar())
                        return Identifier();
                    return EmitToken(TokenType.Invalid);

            }
        }
        public IEnumerable<Token> Lex()
        {
            while (!reader.IsAtEnd)
            {
                var token = NextToken();
                if (token.Type is TokenType.NewLine or TokenType.Whitespace)
                    continue;
                yield return token;
            }
        }
    }
}
