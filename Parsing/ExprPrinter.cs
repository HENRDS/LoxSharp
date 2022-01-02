using System.Linq;
namespace LoxSharp.Parsing
{
    public class ExprPrinter : IExprVisitor<string>
    {
        public string Parenthesize(string head, params string[] tail) => $"({head} {string.Join(' ', tail)})";
        public string VisitBinary(Expr.Binary binary) =>
            Parenthesize(binary.Operator.Lexeme, binary.Left.Accept(this), binary.Right.Accept(this));

        public string VisitCall(Expr.Call call)
        {
            throw new System.NotImplementedException();
        }

        public string VisitComma(Expr.Comma comma) =>
            Parenthesize(",", comma.Values.Select(v => v.Accept(this)).ToArray());


        public string VisitConditional(Expr.Conditional conditional) =>
            Parenthesize("?", conditional.Condition.Accept(this), conditional.Then.Accept(this), conditional.Else.Accept(this));

        public string VisitGet(Expr.Get get) => $"{get.Object.Accept(this)}.{get.Name}";

        public string VisitGrouping(Expr.Grouping grouping) => Parenthesize("grp", grouping.Expr.Accept(this));

        public string VisitLambda(Expr.Lambda lambda) 
        {
            string parameters = $"[{string.Join(' ', lambda.Parameters.Select(x => x.Lexeme))}]";
            return Parenthesize("fun", parameters, lambda.Body.Accept(this));
        }

        public string VisitLiteral(Expr.Literal literal) =>
            literal.Value switch {
                string s => $"\"{s}\"",
                object o => o.ToString()!,
                null => "nil",
            };

        public string VisitLogic(Expr.Logic logic) =>
            Parenthesize(logic.Operator.Lexeme, logic.Left.Accept(this), logic.Right.Accept(this));

        public string VisitUnary(Expr.Unary unary) => Parenthesize(unary.Operator.Lexeme, unary.Right.Accept(this));

        public string VisitVariable(Expr.Variable variable) => variable.Name.Lexeme;
        public string Print(Expr expr) => expr.Accept(this);

        public string VisitAssign(Expr.Assign assign)
        {
            throw new NotImplementedException();
        }

        public string VisitSet(Expr.Set set)
        {
            throw new NotImplementedException();
        }

        public string VisitThis(Expr.This @this)
        {
            return "this";
        }

        public string VisitBase(Expr.Base @base)
        {
            throw new NotImplementedException();
        }
    }
}