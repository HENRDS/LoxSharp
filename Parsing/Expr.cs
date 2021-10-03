using LoxSharp.Lexing;

namespace LoxSharp.Parsing
{
    public interface IExprVisitor
    {
        public void VisitGrouping(Expr.Grouping grouping);
        public void VisitLambda(Expr.Lambda lambda);
        public void VisitComma(Expr.Comma comma);
        public void VisitBinary(Expr.Binary binary);
        public void VisitLogic(Expr.Logic logic);
        public void VisitAssign(Expr.Assign assign);
        public void VisitCall(Expr.Call call);
        public void VisitAccess(Expr.Access access);
        public void VisitLiteral(Expr.Literal literal);
        public void VisitUnary(Expr.Unary unary);
        public void VisitConditional(Expr.Conditional conditional);
        public void VisitVariable(Expr.Variable variable);
        public void VisitGet(Expr.Get get);
    }
    public interface IExprVisitor<T>
    {
        public T VisitGrouping(Expr.Grouping grouping);
        public T VisitLambda(Expr.Lambda lambda);
        public T VisitComma(Expr.Comma comma);
        public T VisitBinary(Expr.Binary binary);
        public T VisitLogic(Expr.Logic logic);
        public T VisitAssign(Expr.Assign assign);
        public T VisitCall(Expr.Call call);
        public T VisitAccess(Expr.Access access);
        public T VisitLiteral(Expr.Literal literal);
        public T VisitUnary(Expr.Unary unary);
        public T VisitConditional(Expr.Conditional conditional);
        public T VisitVariable(Expr.Variable variable);
        public T VisitGet(Expr.Get get);
    }
    public abstract partial class Expr
    {

        public abstract void Accept(IExprVisitor visitor);
        public abstract T Accept<T>(IExprVisitor<T> visitor);

            public sealed partial class Grouping: Expr
            {
                public Expr Expr { get; set; }
                public Grouping(Expr expr)
                {
                    Expr = expr;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitGrouping(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGrouping(this);
            }
            public sealed partial class Lambda: Expr
            {
                public Token Keyword { get; set; }
                public List<Token> Parameters { get; set; }
                public Expr Body { get; set; }
                public Lambda(Token keyword, List<Token> parameters, Expr body)
                {
                    Keyword = keyword;
                    Parameters = parameters;
                    Body = body;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitLambda(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLambda(this);
            }
            public sealed partial class Comma: Expr
            {
                public List<Expr> Values { get; set; }
                public Comma(List<Expr> values)
                {
                    Values = values;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitComma(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitComma(this);
            }
            public sealed partial class Binary: Expr
            {
                public Expr Left { get; set; }
                public Token Operator { get; set; }
                public Expr Right { get; set; }
                public Binary(Expr left, Token @operator, Expr right)
                {
                    Left = left;
                    Operator = @operator;
                    Right = right;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitBinary(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinary(this);
            }
            public sealed partial class Logic: Expr
            {
                public Expr Left { get; set; }
                public Token Operator { get; set; }
                public Expr Right { get; set; }
                public Logic(Expr left, Token @operator, Expr right)
                {
                    Left = left;
                    Operator = @operator;
                    Right = right;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitLogic(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLogic(this);
            }
            public sealed partial class Assign: Expr
            {
                public Token Name { get; set; }
                public Expr Expr { get; set; }
                public Assign(Token name, Expr expr)
                {
                    Name = name;
                    Expr = expr;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitAssign(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAssign(this);
            }
            public sealed partial class Call: Expr
            {
                public Expr Callee { get; set; }
                public Token Paren { get; set; }
                public List<Expr>? PositionalArguments { get; set; }
                public Dictionary<string, Expr>? NamedArguments { get; set; }
                public Call(Expr callee, Token paren, List<Expr>? positionalArguments, Dictionary<string, Expr>? namedArguments)
                {
                    Callee = callee;
                    Paren = paren;
                    PositionalArguments = positionalArguments;
                    NamedArguments = namedArguments;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitCall(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitCall(this);
            }
            public sealed partial class Access: Expr
            {
                public Expr Left { get; set; }
                public Token Right { get; set; }
                public Access(Expr left, Token right)
                {
                    Left = left;
                    Right = right;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitAccess(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAccess(this);
            }
            public sealed partial class Literal: Expr
            {
                public object? Value { get; set; }
                public Literal(object? value)
                {
                    Value = value;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitLiteral(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLiteral(this);
            }
            public sealed partial class Unary: Expr
            {
                public Token Operator { get; set; }
                public Expr Right { get; set; }
                public Unary(Token @operator, Expr right)
                {
                    Operator = @operator;
                    Right = right;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitUnary(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitUnary(this);
            }
            public sealed partial class Conditional: Expr
            {
                public Expr Condition { get; set; }
                public Expr Then { get; set; }
                public Expr Else { get; set; }
                public Conditional(Expr condition, Expr then, Expr @else)
                {
                    Condition = condition;
                    Then = then;
                    Else = @else;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitConditional(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitConditional(this);
            }
            public sealed partial class Variable: Expr
            {
                public Token Name { get; set; }
                public Variable(Token name)
                {
                    Name = name;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitVariable(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitVariable(this);
            }
            public sealed partial class Get: Expr
            {
                public Expr Object { get; set; }
                public Token Name { get; set; }
                public Get(Expr @object, Token name)
                {
                    Object = @object;
                    Name = name;
                }
                public override void Accept(IExprVisitor visitor) => visitor.VisitGet(this);
                public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGet(this);
            }
    }
}
