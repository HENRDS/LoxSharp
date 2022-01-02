using LoxSharp.Core;
using LoxSharp.Lexing;
namespace LoxSharp.Runtime;

public class LoxInstance : LoxObject
{
    private Dictionary<string, object?> fields;
    public LoxClass Type {get;}
    public LoxInstance(LoxClass type)
    {
        Type = type;
        fields = new Dictionary<string, object?>();
    }
    public object? Get(Token name) 
    {
        if (fields.TryGetValue(name.Lexeme, out var result))
        {
            return result;
        }
        LoxFunction? meth = Type.FindMethod(name.Lexeme);
        if (meth is not null) 
        {
            return meth.Bind(this);
        }
        throw new RuntimeException(name, $"Undefined property {name.Lexeme}");
    }
    public void Set(Token name, object? value)
    {
        fields[name.Lexeme] = value;
    }

    public override string ToString()
    {
        return $"{Type} instance";
    }
}