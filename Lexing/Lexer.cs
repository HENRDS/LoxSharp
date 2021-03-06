using System;
using System.Collections.Generic;

namespace LoxSharp.Lexing
{
    public class Lexer
    {
        public static readonly HashSet<string> Keywords = new() {
            "and", "base", "break", "const", "class", "continue", "else", "enum",
            "false", "for", "fun", "if", "in", "not", "nil", "or", "return",
            "this", "true", "var", "while" };
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
            while (reader.Current.IsIdentifierChar())
                reader.Advance();
            if (Keywords.Contains(reader.Lexeme))
                return EmitToken(Enum.Parse<TokenType>(reader.Lexeme, true));
            return EmitToken(TokenType.Identifier);
        }
        private Token Comment()
        {
            while (reader.Current != '\n')
                reader.Advance();
            return EmitToken(TokenType.Comment);
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
                case '-':
                    if (reader.Match('>'))
                        return EmitToken(TokenType.DashGreater);
                    return EmitToken(TokenType.Minus);
                case '*': return EmitToken(TokenType.Star);
                case '/': return EmitToken(TokenType.Slash);
                case '>': return EmitToken(reader.Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                case '<': return EmitToken(reader.Match('=') ? TokenType.LessEqual : TokenType.Less);
                case '&': return EmitToken(TokenType.Ampersand);
                case ',': return EmitToken(TokenType.Comma);
                case ':': return EmitToken(TokenType.Colon);
                case '.': return EmitToken(TokenType.Dot);
                case ';': return EmitToken(TokenType.Semicolon);
                case '#': return Comment();
                case ' ':
                case '\t':
                    while (reader.Current is ' ' or '\t')
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
                if (token.Type is TokenType.NewLine or TokenType.Whitespace or TokenType.Comment)
                    continue;
                yield return token;
            }
            yield return EmitToken(TokenType.Eof);
        }
    }
}
