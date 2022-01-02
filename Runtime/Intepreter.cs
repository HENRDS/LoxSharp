using LoxSharp.Core;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class Interpreter : IExprVisitor<object?>, IStmtVisitor
    {
        internal Scope Global { get; private set; }
        internal Scope CurrentScope { get; private set; }
        internal Dictionary<Expr, int> Locals {get;}
        public Interpreter()
        {
            Global = new Scope();
            CurrentScope = Global;
            Locals = new();
            Global.SafeDefine("clock", new LoxNative(0, _ => (double)DateTime.Now.TimeOfDay.Milliseconds));
        }

        public void Resolve(Expr expr, int depth)
        {
            Locals[expr] = depth;
        }

        private object? LookupName(Token name, Expr expr)
        {
            if (Locals.TryGetValue(expr, out var distance))
            {
                return CurrentScope.GetAt(name, distance);
            }
            else
            {
                return Global.Get(name);
            }
        }
        public void Interpret(List<Stmt> stmts)
        {
            try
            {
                foreach (Stmt stmt in stmts)
                    Execute(stmt);
            }
            catch (RuntimeException e)
            {
                ReportError(e);
            }
        }
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }
        public object? Evaluate(Expr expr) => expr.Accept(this);
        void ReportError(RuntimeException error)
        {
            Console.WriteLine($"{error.Message} @ {error.Token.Position}");
        }
        public object? VisitBinary(Expr.Binary binary)
        {
            object? left = Evaluate(binary.Left);
            object? right = Evaluate(binary.Right);
            switch (binary.Operator.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! > (double)right!;
                case TokenType.Less:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! < (double)right!;
                case TokenType.GreaterEqual:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! >= (double)right!;
                case TokenType.LessEqual:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! <= (double)right!;
                case TokenType.Equal2:
                    return IsTruthy(left?.Equals(right));
                case TokenType.BangEqual:
                    return !IsTruthy(left?.Equals(right));
                case TokenType.Plus:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! + (double)right!;
                case TokenType.Minus:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! - (double)right!;
                case TokenType.Star:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! * (double)right!;
                case TokenType.Slash:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! / (double)right!;
                case TokenType.Ampersand:
                    CheckNumberOperand<string>(binary.Operator, left, right);
                    return (string)left! + (string)right!;
                default:
                    return null;
            }
        }

        public object? VisitCall(Expr.Call call)
        {
            object? callee = Evaluate(call.Callee);
            if (callee is ILoxCallable callable)
            {
                List<object?> arguments = call.PositionalArguments.Select(Evaluate).ToList();
                if (arguments.Count != callable.Arity)
                    throw new RuntimeException(call.Paren, $"Expected {callable.Arity} arguments for function call, but got {arguments.Count}");
                return callable.Call(this, arguments);
            }
            throw new RuntimeException(call.Paren, "Can only call functions or classes");
        }

        public object? VisitComma(Expr.Comma comma)
        {
            throw new System.NotImplementedException();
        }

        public object? VisitConditional(Expr.Conditional conditional)
        {
            bool condition = IsTruthy(Evaluate(conditional.Condition));
            if (condition)
                return Evaluate(conditional.Then);
            return Evaluate(conditional.Else);
        }

        public object? VisitGet(Expr.Get get)
        {
            object? obj = Evaluate(get.Object);
            if (obj is LoxInstance instance){
                return instance.Get(get.Name);
            }
            throw new RuntimeException(get.Name, "Only instances have properties");
        }

        public object? VisitGrouping(Expr.Grouping grouping) => Evaluate(grouping.Expr);

        public object? VisitLambda(Expr.Lambda lambda)
        {
            throw new System.NotImplementedException();
        }

        public object? VisitLiteral(Expr.Literal literal) => literal.Value;

        public object? VisitLogic(Expr.Logic logic)
        {
            switch (logic.Operator.Type)
            {
                case TokenType.And:
                    if (IsTruthy(Evaluate(logic.Left)))
                        return IsTruthy(Evaluate(logic.Right));
                    return false;
                case TokenType.Or:
                    if (!IsTruthy(Evaluate(logic.Left)))
                        return IsTruthy(Evaluate(logic.Right));
                    return true;
                default:
                    return null;
            }
        }

        public object? VisitUnary(Expr.Unary unary)
        {
            object? right = Evaluate(unary.Right);
            switch (unary.Operator.Type)
            {
                case TokenType.Not:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(unary.Operator, right);
                    return -(double)right!;
                case TokenType.Plus:
                    CheckNumberOperand(unary.Operator, right);
                    return +(double)right!;
                default:
                    return null;
            }
        }
        bool IsTruthy(object? value) =>
            value switch
            {
                null => false,
                bool b => b,
                double d => d != 0,
                _ => true
            };
        void CheckNumberOperand(Token @operator, object? operand)
        {
            if (operand is double)
                return;
            throw new RuntimeException(@operator, "Operand must be a number");
        }
        void CheckNumberOperand<T>(Token @operator, object? operand1, object? operand2)
        {
            if (operand1 is T && operand2 is T)
                return;
            throw new RuntimeException(@operator, "Operands must be a numbers");
        }
        public object? VisitVariable(Expr.Variable variable)
        {
            return CurrentScope.Get(variable.Name);
        }

        public void VisitPrint(Stmt.Print print)
        {
            object? value = Evaluate(print.Expr);
            Console.WriteLine(value);
        }

        public void VisitExpression(Stmt.Expression expression)
        {
            Evaluate(expression.Expr);
        }

        public void VisitVar(Stmt.Var var)
        {
            object? value = null;
            if (var.Initializer != null)
            {
                value = Evaluate(var.Initializer);
            }
            CurrentScope.Define(var.Name, value);
        }

        public object? VisitAssign(Expr.Assign assign)
        {
            object? value = Evaluate(assign.Expr);
            if (Locals.TryGetValue(assign, out var distance))
            {
                CurrentScope.AssginAt(assign.Name, value, distance);
            }
            else
            {
                Global.Assign(assign.Name, value);
            }
            return value;
        }
        internal void ExecuteBlock(List<Stmt> stmts, Scope scope)
        {
            Scope previous = this.CurrentScope;
            try
            {
                this.CurrentScope = scope;
                foreach (Stmt stmt in stmts)
                    Execute(stmt);
            }
            finally
            {
                this.CurrentScope = previous;
            }
        }
        public void VisitBlock(Stmt.Block block)
        {
            ExecuteBlock(block.Statements, new Scope(CurrentScope));
        }

        public void VisitIf(Stmt.If @if)
        {
            if (IsTruthy(Evaluate(@if.Condition)))
                Execute(@if.Then);
            else if (@if.Else is not null)
                Execute(@if.Else);
        }

        public void VisitWhile(Stmt.While @while)
        {
            while (IsTruthy(Evaluate(@while.Condition)))
            {
                try
                {
                    Execute(@while.Body);
                }
                catch (ContinueException)
                {
                    continue;
                }
                catch (BreakException)
                {
                    break;
                }
            }
        }

        public void VisitBreak(Stmt.Break @break)
        {
            if (@break.Condition is not null)
                if (!IsTruthy(Evaluate(@break.Condition)))
                    return;
            throw new BreakException();
        }

        public void VisitContinue(Stmt.Continue @continue)
        {
            if (@continue.Condition is not null)
                if (!IsTruthy(Evaluate(@continue.Condition)))
                    return;
            throw new ContinueException();
        }

        public void VisitFunction(Stmt.Function function)
        {
            LoxFunction callable = new(function, new Scope(CurrentScope), false);
            CurrentScope.Define(function.Name, callable);
        }

         public void VisitReturn(Stmt.Return @return)
        {
            object? value = null;
            if (@return.Value is not null)
                value = Evaluate(@return.Value);  
            throw new ReturnException(value);
        }

        public void VisitClass(Stmt.Class @class)
        {
            object? baseClass = null;
            if (@class.Base is not null)
            {
                baseClass = Evaluate(@class.Base);
                if (baseClass is not LoxClass)
                {
                    throw new RuntimeException(@class.Base.Name, "Base class must be a class");
                }
            }
            CurrentScope.Define(@class.Name, null);
            if (@class.Base is not null)
            {
                CurrentScope = new Scope(CurrentScope);
                CurrentScope.SafeDefine("base", baseClass);
            }
            Dictionary<string, LoxFunction> methods = new();
            foreach(Stmt.Function meth in @class.Methods) 
            {
                var fun = new LoxFunction(meth, CurrentScope, meth.Name.Lexeme == "init");
                methods[meth.Name.Lexeme] = fun;
            }
            var cls = new LoxClass(@class.Name.Lexeme, (LoxClass?)baseClass, methods);    
            if (@class.Base is not null)
            {
                CurrentScope = CurrentScope.Parent!;
            }
            CurrentScope.Assign(@class.Name, cls);


        }

        public object? VisitSet(Expr.Set set)
        {
            object? obj = Evaluate(set.Object);
            if (obj is LoxInstance instance)
            {
                object? value = Evaluate(set.Value);
                instance.Set(set.Name, value);
                return value;
            }
            throw new RuntimeException(set.Name, "Only instances have properties");
        }

        public object? VisitThis(Expr.This @this)
        {
            return LookupName(@this.Keyword, @this);
        }

        public object? VisitBase(Expr.Base @base)
        {
            int distance =  Locals[@base];
            var baseClass = (LoxClass)CurrentScope.GetAt("base", distance)!;
            var instance = (LoxInstance)CurrentScope.GetAt("this", distance - 1)!;
            LoxFunction? method = baseClass.FindMethod(@base.Name.Lexeme);
            if (method is null) 
            {
                throw new RuntimeException(@base.Name, $"Undefined method {@base.Name.Lexeme}");
            }
            return method.Bind(instance);
        }
    }
}