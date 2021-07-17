namespace LoxSharp.Lexing
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char c) => c is >= '0' and <= '9';
        public static bool IsIdentifierStartChar(this char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';
        public static bool IsIdentifierChar(this char c) => c.IsIdentifierStartChar() || c.IsDigit();
    }
}
