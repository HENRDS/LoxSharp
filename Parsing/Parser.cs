using LoxSharp.Lexing;
using LoxSharp.Core;

namespace LoxSharp.Parsing
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
        }
        private Token Peek(int offset = 0) => tokens[current + offset];
        private bool IsAtEnd => Peek().Type == TokenType.Eof;

        private Token Advance()
        {
            if (!IsAtEnd)
                current++;
            return Peek(-1);
        }

        private bool Check(params TokenType[] types) => !IsAtEnd && types.Contains(Peek().Type);
        private bool Match(params TokenType[] types)
        {
            if (types.Contains(tokens[current].Type))
            {
                Advance();
                return true;
            }
            return false;
        }

        private ParseError Error(string message, Token? tk = null)
        {
            if (tk is null)
                return new ParseError(message);
            return new ParseError($"{message} at {tk.Position}");
        }
        private Token Consume(params TokenType[] types)
        {
            if (Match(types))
                return Peek(-1);
            throw new Exception($"Expected one of {types} but found {Peek().Type}");
        }
        private void Synchronize() 
        {
            Advance();
            while(!IsAtEnd) 
            {
                if (Peek(-1).Type == TokenType.Semicolon) return;
                switch(Peek().Type) 
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Return:
                        return;
                }    
                Advance();
            }
        }
        public List<Stmt> Parse()
        {
            List<Stmt> stmts = new();
            while(!IsAtEnd) 
            {
                Stmt? decl = Declaration();
                if (decl != null)
                    stmts.Add(decl);

            }
            return stmts;
        }
        private Stmt? Declaration() 
        {
            try 
            {
                if (Check(TokenType.Var))
                    return VarStmt();
                
                return Statement();
            } 
            catch(RuntimeException)
            {
                Synchronize();
                return null;
            }
        }
        private Stmt Statement()
        {
            if (Check(TokenType.Identifier)) {
                if (Peek().Lexeme == "print") 
                {
                    return PrintStmt();
                }
            }
            if (Check(TokenType.Var))
                return VarStmt();
            return ExpressionStmt();

        }
        private Stmt ExpressionStmt()
        {
            Expr expr = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Expression(expr);
        }
        private Stmt PrintStmt() 
        {
            Consume(TokenType.Identifier);
            Consume(TokenType.LeftParen);
            Expr expr = Expression();
            Consume(TokenType.RightParen);
            Consume(TokenType.Semicolon);

            return new Stmt.Print(expr);
        }
        private Stmt VarStmt()
        {
            Consume(TokenType.Var);
            Token name = Consume(TokenType.Identifier);
            Consume(TokenType.Equal);
            Expr init = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Var(name, init);
        }
        private Expr Expression() => Comma();
        private Expr Assignment() 
        {
            Expr lhs = Comma();
            if (Match(TokenType.Equal))
            {
                Token equals = Peek(-1);
                Expr value = Assignment();
                if (lhs is Expr.Variable var) 
                {
                    return new Expr.Assign(var.Name, value);
                }
                throw Error("Invalid assignment target", equals);
            }
            return lhs;
        }
        private Expr Comma()
        {
            List<Expr> exprs = new();
            do
            {
                exprs.Add(Lambda());
            }
            while(Match(TokenType.Comma));
            return exprs.Count == 1 ? exprs.First() : new Expr.Comma(exprs);
        }
        private Expr Lambda()
        {
            if (!Match(TokenType.Fun))
                return Conditional();
            var kwd = Peek(-1);
            Consume(TokenType.LeftParen);
            List<Token> parameters = new();
            if (Match(TokenType.Identifier))
            {
                parameters.Add(Peek(-1));
                while (Match(TokenType.Semicolon))
                {
                    parameters.Add(Consume(TokenType.Identifier));
                }
            }
            Consume(TokenType.RightParen);
            Consume(TokenType.DashGreater);
            var body = Conditional();
            return new Expr.Lambda(kwd, parameters, body);
        }
        private Expr Conditional()
        {
            if (!Match(TokenType.If))
                return LogicalOr();
            var condition = LogicalOr();
            Consume(TokenType.DashGreater);
            var then = Conditional();
            Consume(TokenType.Else);
            var @else = Conditional();
            return new Expr.Conditional(condition, then, @else);
        }
        private Expr LeftAssociativeBinaryRule(
            Func<Expr> subRule,
            Func<Expr, Token, Expr, Expr> accumulator,
            params TokenType[] separators)
        {
            var lhs = subRule();
            while (Match(separators))
            {
                Token op = Peek(-1);
                var rhs = subRule();
                lhs = accumulator(lhs, op, rhs);
            }
            return lhs;
        }
        private Expr LeftAssociativeBinaryExpr(Func<Expr> subRule, params TokenType[] separators) =>
            LeftAssociativeBinaryRule(subRule, (l, o, r) => new Expr.Binary(l, o, r), separators);

        private Expr LeftAssociativeLogicExpr(Func<Expr> subRule, params TokenType[] separators) =>
            LeftAssociativeBinaryRule(subRule, (l, o, r) => new Expr.Logic(l, o, r), separators);

        private Expr LogicalOr() => LeftAssociativeLogicExpr(LogicalAnd, TokenType.Or);
        private Expr LogicalAnd() => LeftAssociativeLogicExpr(Equality, TokenType.And);
        private Expr Equality() => LeftAssociativeBinaryExpr(Comparison, TokenType.Equal2, TokenType.BangEqual);
        private Expr Comparison() =>
            LeftAssociativeBinaryExpr(Cat, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual);
        private Expr Cat() => LeftAssociativeBinaryExpr(Add, TokenType.Ampersand);
        private Expr Add() => LeftAssociativeBinaryExpr(Mul, TokenType.Plus, TokenType.Minus);
        private Expr Mul() => LeftAssociativeBinaryExpr(Unary, TokenType.Star, TokenType.Slash);

        private Expr Unary()
        {
            if (!Match(TokenType.Not, TokenType.Minus, TokenType.Plus))
                return Primary();
            Token op = Peek(-1);
            var rhs = Unary();
            return new Expr.Unary(op, rhs);
        }
        private Expr Call()
        {
            var lhs = Access();
            // while(Match(TokenType.LeftParen))
            // {
            //     while(!Check(TokenType.RightParen)) 
            //     {
            //         if (Check(TokenType.Identifier)) {
            //             if (Peek(2).Type == TokenType.Equal) {
            //                 Token name = Consume(TokenType.Identifier);

            //             }
            //         }
                    
            //     }
            //     Consume(TokenType.RightParen);
            // }
            return lhs;
        }
        private Expr Access()
        {
            var lhs = Primary();
            while(Match(TokenType.Dot))
                lhs = new Expr.Access(lhs, Consume(TokenType.Identifier));
            return lhs;
        }
        private Expr Primary()
        {
            if (Match(TokenType.True)) return new Expr.Literal(true);
            if (Match(TokenType.False)) return new Expr.Literal(false);
            if (Match(TokenType.StringLit, TokenType.NumberLit, TokenType.Nil))
                return new Expr.Literal(Peek(-1).Literal);
            if (Match(TokenType.Identifier)) 
                return new Expr.Variable(Peek(-1));
            if (Match(TokenType.LeftParen))
            {
                var body = Expression();
                Consume(TokenType.RightParen);
                return new Expr.Grouping(body);
            }
            throw Error("Expected expression");

        }
    }
}
