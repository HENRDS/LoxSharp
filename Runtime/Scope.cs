using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LoxSharp.Core;
using LoxSharp.Lexing;

namespace LoxSharp.Runtime
{
    class Scope: IEnumerable<KeyValuePair<string, object?>>, IEnumerable
    {
        private readonly Dictionary<string, object?> values;
        public Scope? Parent {get;}

        public IReadOnlyCollection<string> Keys => values.Keys;

        public IReadOnlyCollection<object?> Values => values.Values;

        public int Count => values.Count;

        public bool IsReadOnly => false;

        public object? this[Token key] { get => values[key.Lexeme]; set => Define(key, value); }
        public object? this[string key] { get => values[key]; set => SafeDefine(key, value); }

        public Scope(Scope? parent = null) 
        {
            Parent = parent;
            values = new(); 
        }
        
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
        public void SafeDefine(string name, object? value) 
        {
            values.Add(name, value);
        }
        public void Define(Token name, object? value) 
        {
            values.Add(name.Lexeme, value);
        }
        public void Assign(Token name, object? value)
        {
            if (values.ContainsKey(name.Lexeme)) 
            {
                values.Add(name.Lexeme, value);
                return;
            }
            if (Parent != null)
                Parent.Assign(name, value);
                
            throw new RuntimeException(name, $"Undeclared identifier {name.Lexeme}");
        }
        public object? Get(Token name) 
        {
            if (values.TryGetValue(name.Lexeme, out var value))
                return value;
            if (Parent != null)
                return Parent.Get(name);
            throw new RuntimeException(name, $"Undeclared identifier {name.Lexeme}");
        }
        public bool TryGetValue(string name, out object? value) 
        {
            if(values.TryGetValue(name, out value))
                return true;
            if (Parent != null)
                return Parent.TryGetValue(name, out value); 
            return false;
        }
    }
}