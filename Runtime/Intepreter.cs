using LoxSharp.Core;
using LoxSharp.Lexing;
using LoxSharp.Parsing;

namespace LoxSharp.Runtime
{
    public class Interpreter : IExprVisitor<object?>, IStmtVisitor
    {
        readonly Scope scope = new();
        public void Interpret(List<Stmt> stmts) 
        {
            try 
            {
                foreach(Stmt stmt in stmts)
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
        private object? Evaluate(Expr expr) => expr.Accept(this);
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
                    return (double)left! >  (double)right!;
                case TokenType.Less:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! <  (double)right!;
                case TokenType.GreaterEqual:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! >= (double)right!;
                case TokenType.LessEqual:
                    CheckNumberOperand<double>(binary.Operator, left, right);
                    return (double)left! <= (double)right!;
                case TokenType.Equal2:
                    return left == right;
                case TokenType.BangEqual:
                    return left != right;
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public object? VisitGrouping(Expr.Grouping grouping) => Evaluate(grouping.Expr);

        public object? VisitLambda(Expr.Lambda lambda)
        {
            throw new System.NotImplementedException();
        }

        public object? VisitLiteral(Expr.Literal literal) => literal.Value;

        public object? VisitLogic(Expr.Logic logic)
        {
            switch(logic.Operator.Type)
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
                    return - (double)right!;
                case TokenType.Plus:
                    CheckNumberOperand(unary.Operator, right);
                    return + (double)right!;
                default:
                    return null;
            }
        }
        bool IsTruthy(object? value) =>
            value switch {
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
            return scope.Get(variable.Name);
        }

        public object? VisitAccess(Expr.Access access)
        {
            throw new NotImplementedException();
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
            scope.Define(var.Name, value);
        }

        public object? VisitAssign(Expr.Assign assign)
        {
            object? value = Evaluate(assign.Expr);
            scope.Assign(assign.Name, value);
            return value;
        }
    }
}