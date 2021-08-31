using System.Collections.Generic;
using LoxSharp.Lexing;
using System.Linq;
using System;
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

        private ParseError Error(string message)
        {
            return new ParseError(message);
        }
        private Token Consume(params TokenType[] types)
        {
            if (Match(types))
                return Peek(-1);
            throw new Exception($"Expected one of {types} but found {Peek().Type}");
        }
        public Expr Parse()
        {
            return Expression();
        }
        private Expr Expression() => Comma();

        private Expr Comma()
        {
            List<Expr> exprs = new();
            do
            {
                exprs.Add(Conditional());
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
            LeftAssociativeBinaryExpr(Add, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual);
        private Expr BitwiseOr() => LeftAssociativeBinaryExpr(BitwiseAnd, TokenType.Pipe);
        private Expr BitwiseAnd() => LeftAssociativeBinaryExpr(BitShift, TokenType.Ampersand);
        private Expr BitShift() => LeftAssociativeBinaryExpr(Add, TokenType.Shr, TokenType.Shl);
        private Expr Add() => LeftAssociativeBinaryExpr(Mul, TokenType.Plus, TokenType.Minus);
        private Expr Mul() => LeftAssociativeBinaryExpr(Unary, TokenType.Star, TokenType.Slash);

        private Expr Unary()
        {
            if (!Match(TokenType.Not, TokenType.Minus, TokenType.Plus, TokenType.Tilde))
                return Primary();
            Token op = Peek(-1);
            var rhs = Unary();
            return new Expr.Unary(op, rhs);
        }
        private Expr Call()
        {
            throw new NotImplementedException();
        }
        private Expr Access()
        {
            throw new NotImplementedException();
        }
        private Expr Primary()
        {
            if (Match(TokenType.True)) return new Expr.Literal(true);
            if (Match(TokenType.False)) return new Expr.Literal(false);
            if (Match(TokenType.StringLit, TokenType.NumberLit))
                return new Expr.Literal(Peek(-1).Literal!);
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
