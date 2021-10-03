using LoxSharp.Lexing;

namespace LoxSharp.Parsing
{
    public interface IStmtVisitor
    {
        public void VisitPrint(Stmt.Print print);
        public void VisitVar(Stmt.Var var);
        public void VisitExpression(Stmt.Expression expression);
    }
    public interface IStmtVisitor<T>
    {
        public T VisitPrint(Stmt.Print print);
        public T VisitVar(Stmt.Var var);
        public T VisitExpression(Stmt.Expression expression);
    }
    public abstract partial class Stmt
    {

        public abstract void Accept(IStmtVisitor visitor);
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

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
            public sealed partial class Var: Stmt
            {
                public Token Name { get; set; }
                public Expr Initializer { get; set; }
                public Var(Token name, Expr initializer)
                {
                    Name = name;
                    Initializer = initializer;
                }
                public override void Accept(IStmtVisitor visitor) => visitor.VisitVar(this);
                public override T Accept<T>(IStmtVisitor<T> visitor) => visitor.VisitVar(this);
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
    }
}
