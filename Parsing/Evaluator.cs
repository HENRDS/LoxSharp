using LoxSharp.Core;
using LoxSharp.Lexing;
namespace LoxSharp.Parsing
{
    public class Evaluator : IExprVisitor<object?>
    {
        public object? VisitBinary(Expr.Binary binary)
        {
            object? left = binary.Left.Accept(this);
            object? right = binary.Right.Accept(this);
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
            bool condition = IsTruthy(conditional.Condition.Accept(this));
            if (condition)
                return conditional.Then.Accept(this);
            return conditional.Else.Accept(this);
       }

        public object? VisitGet(Expr.Get get)
        {
            throw new System.NotImplementedException();
        }

        public object? VisitGrouping(Expr.Grouping grouping) => grouping.Expr.Accept(this);

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
                    if (IsTruthy(logic.Left.Accept(this)))
                        return IsTruthy(logic.Right.Accept(this));
                    return false;
                case TokenType.Or:
                    if (!IsTruthy(logic.Left.Accept(this)))
                        return IsTruthy(logic.Right.Accept(this));
                    return true;
                default:
                    return null;
            }
        }

        public object? VisitUnary(Expr.Unary unary)
        {
            object? right = unary.Right.Accept(this);
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
            throw new System.NotImplementedException();
        }
    }
}