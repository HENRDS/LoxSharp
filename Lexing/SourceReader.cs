using System;
using System.Linq;
using System.IO;

namespace LoxSharp.Lexing
{
    public class SourceReader: IDisposable
    {
        public const char InvalidChar = '\xff';
        private readonly StreamReader reader;
        public int Absolute { get; set; }
        public string Buffer { get; set; }
        public int BufferPos { get; set; }
        public int LineStart { get; set; }
        public int Line { get; set; }
        public int LexemeStart { get; set; }
        public SourceReader(Stream stream)
        {
            reader = new StreamReader(stream);
            Buffer = "";
            BufferPos = 0;
            LineStart = 0;
            Line = 1;
            LexemeStart = 0;
            Absolute = 0;
            FillBuffer();
        }
        public static SourceReader FromString(string code)
        {
            var stream = new MemoryStream();
            var sr = new StreamWriter(stream);
            sr.Write(code);
            return new SourceReader(stream);
        }
        public static SourceReader FromFile(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            return new SourceReader(fs);
        }
        protected void FillBuffer()
        {
            Buffer = Buffer[LexemeStart..] + reader.ReadLine() + "\n";
            BufferPos -= LexemeStart;
            LexemeStart = 0;
        }

        public SourcePosition Position => new(Line, LexemeStart - LineStart, Absolute);
        public void HandleNewLine()
        {
            if (IsAtEnd)
                return;
            // Advance();
            Line += 1;
            FillBuffer();
            Sync();
        }
        public void Dispose()
        {
            reader.Dispose();
        }
        public void Advance(int count = 1)
        {
            BufferPos += count;
        }
        public char Peek(int offset = 1)
        {
            int index = BufferPos + offset;
            if (index >= Buffer.Length)
                return '\0';
            return Buffer[index];
        }
        public bool IsAtEnd => reader.EndOfStream && BufferPos >= Buffer.Length;
        public void Sync()
        {
            Absolute += BufferPos - LexemeStart;
            LexemeStart = BufferPos;
        }
        public bool Match(params char[] options)  {
            if (options.Contains(Current))
            {
                Advance();
                return true;
            }
            return false;
        }
        public char Current => Peek(0);
        public string Lexeme => Buffer[LexemeStart..BufferPos];

    }

}
