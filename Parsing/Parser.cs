using LoxSharp.Lexing;
using LoxSharp.Core;

namespace LoxSharp.Parsing
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current;
        private bool isInsideLoop;
        private bool isInsideFunction;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            current = 0;
            isInsideLoop = false;
            isInsideFunction = false;
        }
        private Token Peek(int offset = 0) => tokens[current + offset];
        private bool IsAtEnd => Peek().Type == TokenType.Eof;

        private Token Advance()
        {
            if (!IsAtEnd)
                current++;
            return Peek(-1);
        }
        private Token Recede()
        {
            if (current > 0)
                current--;
            return Peek(1);
        }

        private bool Check(params TokenType[] types) => !IsAtEnd && types.Contains(tokens[current].Type);
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
                tk = Peek();
            return new ParseError($"{message} at {tk.Position}");
        }
        private Token Consume(params TokenType[] types)
        {
            if (Match(types))
                return Peek(-1);
            var typesStr = string.Join(',', types.Select(t => t.ToString()));
            throw new Exception($"Expected one of {{{typesStr}}} but found {Peek().Type} at {tokens[current].Position}");
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
                if (Match(TokenType.Class))
                    return ClassDecl();
                if (Match(TokenType.Var))
                    return VarDecl();
                if (Match(TokenType.Fun))
                    return FunDecl();
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
            if (Match(TokenType.Identifier)) {
                if (Peek(-1).Lexeme == "print") 
                {
                    return PrintStmt();
                }
            }
            if (Check(TokenType.LeftBrace))
                return BlockStmt();
            if (Match(TokenType.If))
                return IfStmt();
            if (Match(TokenType.While))
                return WhileStmt();
            if (Match(TokenType.Return))
                return ReturnStmt();
            if (Match(TokenType.Break))
                return BreakStmt();
            if (Match(TokenType.Continue))
                return ContinueStmt();
            if (Match(TokenType.For))
                return ForStmt();

            return ExpressionStmt();
        }
        private Stmt ReturnStmt() 
        {
            Token kwd = Peek(-1);
            if (!isInsideFunction)
                throw Error("Cannot call return outside a function");
            Expr? expr = null;
            if (!Check(TokenType.Semicolon))
            {
                expr = Expression();
            }
            Consume(TokenType.Semicolon);
            return new Stmt.Return(kwd, expr);
        }
        private Stmt ClassDecl()
        {
            Token name = Consume(TokenType.Identifier);
            Consume(TokenType.LeftBrace);
            List<Stmt.Function> methods = new();
            while (!Check(TokenType.RightBrace) && !IsAtEnd)
            {
                methods.Add((Stmt.Function)FunDecl());
            }
            Consume(TokenType.RightBrace);
            return new Stmt.Class(name, methods);
        }
        private Stmt FunDecl() 
        {
            if (!Check(TokenType.Identifier))
            {
                Recede();
                return new Stmt.Expression(Lambda());
            }
            Token name = Consume(TokenType.Identifier);
            Consume(TokenType.LeftParen);
            List<Token> parameters = new();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    parameters.Add(Consume(TokenType.Identifier));
                } while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen);
            bool isInsideFunction = this.isInsideFunction;
            try
            { 
                this.isInsideFunction = true;
                Stmt body = BlockStmt();
                return new Stmt.Function(name, parameters, body); 
            }
            finally
            {
                this.isInsideFunction = isInsideFunction;
            }
        }
        private Stmt ForStmt() 
        {
            Consume(TokenType.LeftParen);
            Stmt? init;
            if (Match(TokenType.Semicolon))
                init = null;
            else if (Match(TokenType.Var))
                init = VarDecl();
            else 
                init = ExpressionStmt();
            Expr? condition = null;
            if (Check(TokenType.Semicolon))
                condition = Expression();
            Consume(TokenType.Semicolon);
            Expr? incr = null;
            if (!Check(TokenType.RightParen))
                incr = Expression();
            Consume(TokenType.RightParen);
            Stmt body = Statement();
            
            if (incr is not null) 
            {
                body = new Stmt.Block(
                    new List<Stmt>
                    {
                        body, 
                        new Stmt.Expression(incr) 
                    }
                );
            }
            if (condition is null) 
                condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);
            if (init is not null)
            {
                body = new Stmt.Block(
                    new List<Stmt> 
                    {
                        init,
                        body
                    }
                );
            }
            return body;
        }
        private Stmt BreakStmt()
        {
            if (!isInsideLoop)
                throw Error("Cannot use break outside loop");
            Consume(TokenType.Break);
            Expr? condition = null;
            if (Match(TokenType.If))
                condition = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Break(condition);
        }
        private Stmt ContinueStmt()
        {
            if (!isInsideLoop)
                throw Error("Cannot use continue outside loop");
            Consume(TokenType.Continue);
            Expr? condition = null;
            if (Match(TokenType.If))
                condition = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Continue(condition);
        }
        private Stmt WhileStmt()
        {
            bool isInsideLoop = this.isInsideLoop;
            try 
            {
                this.isInsideLoop = true;
                Consume(TokenType.LeftParen);
                Expr condition = Expression();
                Consume(TokenType.RightParen);
                Stmt body = Statement();
                return new Stmt.While(condition, body);
            } 
            finally
            {
                this.isInsideLoop = isInsideLoop;
            }
        }
        private Stmt IfStmt()
        {

            Consume(TokenType.LeftParen);
            Expr condition = Expression();
            Consume(TokenType.RightParen);
            Stmt then = Statement();
            Stmt? @else = null;
            if (Match(TokenType.Else))
                @else = Statement();
            return new Stmt.If(condition, then, @else);
        }
        private Stmt BlockStmt()
        {
            Consume(TokenType.LeftBrace);
            List<Stmt> stmts = new();
            while (!Check(TokenType.RightBrace) && !IsAtEnd)
            {
                Stmt? decl = Declaration();
                if (decl != null)
                    stmts.Add(decl);
            }
            Consume(TokenType.RightBrace);
            return new Stmt.Block(stmts);
        }
        private Stmt ExpressionStmt()
        {
            Expr expr = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Expression(expr);
        }
        private Stmt PrintStmt() 
        {
            Consume(TokenType.LeftParen);
            Expr expr = Expression();
            Consume(TokenType.RightParen);
            Consume(TokenType.Semicolon);

            return new Stmt.Print(expr);
        }
        private Stmt VarDecl()
        {
            Token name = Consume(TokenType.Identifier);
            Consume(TokenType.Equal);
            Expr init = Expression();
            Consume(TokenType.Semicolon);
            return new Stmt.Var(name, init);
        }
        private Expr Expression() => Assignment();
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
                else if (lhs is Expr.Get get) 
                {
                    return new Expr.Set(get.Object, get.Name, value);
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
            var condition = LogicalOr();
            
            if (Match(TokenType.If)) {
                var then = Conditional();
                Consume(TokenType.Else);
                var @else = Conditional();
                return new Expr.Conditional(condition, then, @else);
            }
            return condition;
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
                return Call();
            Token op = Peek(-1);
            var rhs = Unary();
            return new Expr.Unary(op, rhs);
        }
        private Expr FinishCall(Expr callee)
        {
            List<Expr> args = new();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (args.Count >= 255)
                        throw Error("Cannot have more than 255 arguments in a function call");
                    args.Add(Expression());
                } while (Match(TokenType.Comma));
            }
            Token paren = Consume(TokenType.RightParen);
            return new Expr.Call(callee, paren, args, null);    
        }
        private Expr Call()
        {
            var expr = Primary();
            while (true) 
            {
                if (Match(TokenType.LeftParen))
                {
                    expr = FinishCall(expr);   
                }
                else if (Match(TokenType.Dot))
                {
                    Token name = Consume(TokenType.Identifier);
                    expr = new Expr.Get(expr, name);
                }
                else
                {
                    break;
                }
            }
            return expr;
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
            if (Match(TokenType.This))
                return new Expr.This(Peek(-1));
            throw Error("Expected expression");

        }
    }
}
