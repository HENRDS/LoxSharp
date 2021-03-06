namespace LoxSharp.Runtime;

public class LoxClass: LoxObject, ILoxCallable
{
    public string Name {get;}
    public LoxClass? Base {get;}
    private readonly Dictionary<string, LoxFunction> methods;

    public int Arity 
    {
        get 
        {
            LoxFunction? init = FindMethod("init");
            if (init == null)
                return 0;
            return init.Arity;
        }
    }

    public LoxClass(string name, LoxClass? @base, Dictionary<string, LoxFunction> methods)
    {
        Name = name;
        Base = @base;
        this.methods = methods;
    }

    public override string ToString()
    {
        return Name;
    }

    public LoxFunction? FindMethod(string name) 
    {
        if (methods.TryGetValue(name, out var meth))
        {
            return meth;
        }
        if (Base is not null)
        {
            return Base.FindMethod(name);
        }
        return null;
    }

    public object? Call(Interpreter interpreter, params object?[] arguments) => Call(interpreter, arguments.AsEnumerable());

    public object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
    {
        var instance = new LoxInstance(this);
        LoxFunction? initializer = FindMethod("init");
        if (initializer is not null)
            initializer.Bind(instance).Call(interpreter, arguments);
        return instance;
    }
}