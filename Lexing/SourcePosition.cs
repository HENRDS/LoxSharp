namespace LoxSharp.Lexing
{
    public class SourcePosition
    {
        public SourcePosition(int line, int column, int absolute)
        {
            Line = line;
            Column = column;
            Absolute = absolute;

        }
        public int Line { get; set; }
        public int Column { get; set; }
        public int Absolute { get; set; }

        public override string ToString() => $"[{Line}, {Column}; {Absolute}]";
    }
}
