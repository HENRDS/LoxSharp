namespace LoxSharp.Lexing
{
    public enum TokenType
    {
        Invalid, NewLine, Whitespace, Comment,
        LeftParen, RightParen, LeftBracket, RightBracket, LeftBrace, RightBrace,
        Plus, Minus, Star, Slash, Ampersand, Equal, Equal2, Less, Greater, LessEqual, GreaterEqual,
        BangEqual, Comma, Colon, Semicolon, Dot, DashGreater,
        StringLit, NumberLit, Identifier,

        And, Break, Const, Class, Continue, Else, Enum, False, For, Fun, If, In, Not, Nil, Or, Return,
        True, Var, While,

        Eof
    }
    public class Token
    {
        public readonly TokenType Type;
        public readonly SourcePosition Position;
        public readonly string Lexeme;
        public readonly object? Literal;

        public Token(TokenType type, SourcePosition position, string lexeme, object? literal)
        {
            Type = type;
            Position = position;
            Lexeme = lexeme;
            Literal = literal;
        }
        public override string ToString()
        {
            var lit = Type switch {
                TokenType.StringLit => $"\"{Literal}\"",
                TokenType.NumberLit => $"{Literal}",
                _ => ""
            };

            return $"<Token {Type} {Position} '{Lexeme}' {lit}>";
        }
    }
}
