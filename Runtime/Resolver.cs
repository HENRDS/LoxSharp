using LoxSharp.Core;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{

    public class Resolver : IExprVisitor, IStmtVisitor
    {
        private enum FunctionType 
        {
            None,
            Function,
            Initializer,
            Method
        }
        private enum ClassType 
        {
            None,
            Class
        }
        private readonly Stack<Dictionary<string, bool>> scopes;
        private readonly Interpreter Interpreter;
        private FunctionType functionType;
        private ClassType classType;
        public Resolver(Interpreter interpreter)
        {
            Interpreter = interpreter;
            functionType = FunctionType.None;
            classType = ClassType.None;
            scopes = new();
        }
        private void BeginScope() => scopes.Push(new Dictionary<string, bool>());
        private void EndScope() => scopes.Pop();
        public void Resolve(List<Stmt> statements) => statements.ForEach(Resolve);
        private void Resolve(Stmt stmt) => stmt.Accept(this);
        private void Resolve(Expr expr) => expr.Accept(this);
        private void ResolveLocal(Expr expr, Token name) 
        {
            foreach(var (i, scope) in scopes.Select((x, i) => (i,x)))
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    Interpreter.Resolve(expr, i);
                    return;
                }
            }
        }
        private void ResolveFunction(Stmt.Function function, FunctionType type) 
        {
            FunctionType enclosingFunctionType = functionType;
            functionType = type;
            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
            functionType = enclosingFunctionType;
        }
        private void Declare(Token name) 
        {
            if (!scopes.Any()) 
                return;
            var scope = scopes.Peek();
            scope.Add(name.Lexeme, false);
        }
        private void Define(Token name)
        {
            if (!scopes.Any())
                return;
            var scope = scopes.Peek();
            scope[name.Lexeme] = true;
        }

        public void VisitAssign(Expr.Assign assign)
        {
            Resolve(assign.Expr);
            ResolveLocal(assign, assign.Name);
        }

        public void VisitBinary(Expr.Binary binary)
        {
            Resolve(binary.Left);
            Resolve(binary.Right);
        }

        public void VisitBlock(Stmt.Block block)
        {
            BeginScope();
            Resolve(block.Statements);
            EndScope();
        }

        public void VisitBreak(Stmt.Break @break)
        {
            if (@break.Condition is not null)
                Resolve(@break.Condition);
        }

        public void VisitCall(Expr.Call call)
        {
            Resolve(call.Callee);
            foreach (var arg in call.PositionalArguments)
            {
                Resolve(arg);
            }
        }

        public void VisitComma(Expr.Comma comma)
        {
            foreach (var expr in comma.Values) 
            {
                Resolve(expr);
            }
        }

        public void VisitConditional(Expr.Conditional conditional)
        {
            Resolve(conditional.Then);
            Resolve(conditional.Else);
        }

        public void VisitContinue(Stmt.Continue @continue)
        {
            if (@continue.Condition is not null)
                Resolve(@continue.Condition);
        }

        public void VisitExpression(Stmt.Expression expression)
        {
            Resolve(expression.Expr);
        }

        public void VisitFunction(Stmt.Function function)
        {
            Declare(function.Name);
            Define(function.Name);
            ResolveFunction(function, FunctionType.Function);
        }

        public void VisitGet(Expr.Get get)
        {
            Resolve(get.Object);
        }

        public void VisitGrouping(Expr.Grouping grouping)
        {
            Resolve(grouping.Expr);
        }

        public void VisitIf(Stmt.If @if)
        {
            Resolve(@if.Condition);
            Resolve(@if.Then);
            if (@if.Else is not null)
                Resolve(@if.Else);
        }

        public void VisitLambda(Expr.Lambda lambda)
        {
            BeginScope();
            foreach (var param in lambda.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(lambda.Body);
            EndScope();
        }

        public void VisitLiteral(Expr.Literal literal)
        {
        }

        public void VisitLogic(Expr.Logic logic)
        {
            Resolve(logic.Left);
            Resolve(logic.Right);
        }

        public void VisitPrint(Stmt.Print print)
        {
            Resolve(print.Expr);
        }

        public void VisitReturn(Stmt.Return @return)
        {
            if (@return.Value is not null)
            {
                if (functionType == FunctionType.Initializer)
                    throw new RuntimeException(@return.Keyword, "Cannot return from object initializer");
                Resolve(@return.Value);
            }
        }

        public void VisitUnary(Expr.Unary unary)
        {
            Resolve(unary.Right);
        }

        public void VisitVar(Stmt.Var var)
        {
            Declare(var.Name);
            if (var.Initializer is not null)
                Resolve(var.Initializer);
            Define(var.Name);
        }

        public void VisitVariable(Expr.Variable variable)
        {
            if (scopes.Any() && scopes.Peek().ContainsKey(variable.Name.Lexeme) && !scopes.Peek()[variable.Name.Lexeme])
            {
                throw new RuntimeException(variable.Name, "Cannot read local variable in it's own initializer");
            }
            ResolveLocal(variable, variable.Name);
        }

        public void VisitWhile(Stmt.While @while)
        {
            Resolve(@while.Condition);
            BeginScope();
            Resolve(@while.Body);
            EndScope();
        }

        public void VisitClass(Stmt.Class @class)
        {
            ClassType enclosingClassType = classType;
            classType = ClassType.Class;
            Declare(@class.Name);
            Define(@class.Name);
            BeginScope();
            scopes.Peek()["this"] = true;
            foreach(Stmt.Function meth in @class.Methods)
            {
                FunctionType declaration = meth.Name.Lexeme == "init" ? FunctionType.Initializer : FunctionType.Method;
                ResolveFunction(meth, declaration);
            }
            EndScope();
            classType = enclosingClassType;
        }

        public void VisitSet(Expr.Set set)
        {
            Resolve(set.Value);
            Resolve(set.Object);
        }

        public void VisitThis(Expr.This @this)
        {
            if (classType == ClassType.None)
                throw new RuntimeException(@this.Keyword, "Can't use this outside class");
            ResolveLocal(@this, @this.Keyword);
        }
    }

}