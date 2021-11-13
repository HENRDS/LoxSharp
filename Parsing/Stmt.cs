using LoxSharp.Lexing;

namespace LoxSharp.Parsing
{
    public interface IStmtVisitor
    {
        public void VisitFunction(Stmt.Function function);
        public void VisitExpression(Stmt.Expression expression);
        public void VisitBlock(Stmt.Block block);
        public void VisitWhile(Stmt.While @while);
        public void VisitVar(Stmt.Var var);
        public void VisitReturn(Stmt.Return @return);
        public void VisitPrint(Stmt.Print print);
        public void VisitBreak(Stmt.Break @break);
        public void VisitIf(Stmt.If @if);
        public void VisitContinue(Stmt.Continue @continue);
    }
    public interface IStmtVisitor<T>
    {
        public T VisitFunction(Stmt.Function function);
        public T VisitExpression(Stmt.Expression expression);
        public T VisitBlock(Stmt.Block block);
        public T VisitWhile(Stmt.While @while);
        public T VisitVar(Stmt.Var var);
        public T VisitReturn(Stmt.Return @return);
        public T VisitPrint(Stmt.Print print);
        public T VisitBreak(Stmt.Break @break);
        public T VisitIf(Stmt.If @if);
        public T VisitContinue(Stmt.Continue @continue);
    }
    public abstract partial class Stmt
    {

        public abstract void Accept(IStmtVisitor visitor);
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

            public sealed partial class Function: Stmt
            {
                public Token Name { get; set; }
                public List<Token> Parameters { get; set; }
                public Stmt Body { get; set; }
                public Function(Token name, List<Token> parameters, Stmt body)
                {
                    Name = name;
                    Parameters = parameters;
                    Body = body;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitFunction(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitFunction(this);
            }
            public sealed partial class Expression: Stmt
            {
                public Expr Expr { get; set; }
                public Expression(Expr expr)
                {
                    Expr = expr;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitExpression(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitExpression(this);
            }
            public sealed partial class Block: Stmt
            {
                public List<Stmt> Statements { get; set; }
                public Block(List<Stmt> statements)
                {
                    Statements = statements;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitBlock(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitBlock(this);
            }
            public sealed partial class While: Stmt
            {
                public Expr Condition { get; set; }
                public Stmt Body { get; set; }
                public While(Expr condition, Stmt body)
                {
                    Condition = condition;
                    Body = body;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitWhile(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitWhile(this);
            }
            public sealed partial class Var: Stmt
            {
                public Token Name { get; set; }
                public Expr? Initializer { get; set; }
                public Var(Token name, Expr? initializer)
                {
                    Name = name;
                    Initializer = initializer;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitVar(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitVar(this);
            }
            public sealed partial class Return: Stmt
            {
                public Expr? Value { get; set; }
                public Return(Expr? value)
                {
                    Value = value;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitReturn(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitReturn(this);
            }
            public sealed partial class Print: Stmt
            {
                public Expr Expr { get; set; }
                public Print(Expr expr)
                {
                    Expr = expr;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitPrint(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitPrint(this);
            }
            public sealed partial class Break: Stmt
            {
                public Expr? Condition { get; set; }
                public Break(Expr? condition)
                {
                    Condition = condition;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitBreak(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitBreak(this);
            }
            public sealed partial class If: Stmt
            {
                public Expr Condition { get; set; }
                public Stmt Then { get; set; }
                public Stmt? Else { get; set; }
                public If(Expr condition, Stmt then, Stmt? @else)
                {
                    Condition = condition;
                    Then = then;
                    Else = @else;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitIf(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitIf(this);
            }
            public sealed partial class Continue: Stmt
            {
                public Expr? Condition { get; set; }
                public Continue(Expr? condition)
                {
                    Condition = condition;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitContinue(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitContinue(this);
            }
    }
}
