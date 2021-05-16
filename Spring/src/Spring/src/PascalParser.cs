using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.TreeBuilder;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringParser : IParser
    {
        private readonly ILexer _myLexer;

        public SpringParser(ILexer lexer)
        {
            _myLexer = lexer;
        }

        public IFile ParseFile()
        {
            using var def = Lifetime.Define();
            var builder = new PsiBuilder(_myLexer, SpringFileNodeType.Instance, new TokenFactory(), def.Lifetime);
            var fileMark = builder.Mark();
            ParseCompoundStatement(builder);

            builder.Done(fileMark, SpringFileNodeType.Instance, null);
            var file = (IFile) builder.BuildTree();
            return file;
        }

        private static bool ExpectToken(PsiBuilder builder, SpringTokenType tokenType, int mark = -1)
        {
            var tt = GetTokenType(builder);
            if (tt == tokenType) return true;
            var errorStr = $"Expected {tokenType} but {tt} is given.";
            if (mark == -1)
            {
                builder.Error(errorStr);
            }
            else
            {
                builder.Error(mark, errorStr);
            }

            return false;
        }

        private static TokenNodeType GetTokenType(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == null)
            {
                return null;
            }

            if (!(builder.GetTokenType() == SpringTokenType.Whitespace || builder.GetTokenType() == SpringTokenType.Comment)) return tt;
            builder.TryAdvance();
            tt = builder.GetTokenType();
            return tt;
        }

        private void ParseCompoundStatement(PsiBuilder builder)
        {
            ExpectToken(builder, SpringTokenType.Begin);
            var start = builder.Mark();
            builder.TryAdvance();
            ParseStatementList(builder);

            if (ExpectToken(builder, SpringTokenType.End, start))
            {
                builder.Done(start, SpringCompositeNodeType.Statement, null);
            }

            builder.TryAdvance();
        }

        private void ParseStatementList(PsiBuilder builder)
        {
            var tt = GetTokenType(builder);
            ParseStatement(builder);

            while (true)
            {
                tt = GetTokenType(builder);
                if (tt != SpringTokenType.Identifier && tt != SpringTokenType.Begin &&
                    tt != SpringTokenType.ProcedureCall)
                {
                    break;
                }
                ParseStatement(builder);
            }
        }

        private void ParseStatement(PsiBuilder builder)
        {
            var tt = GetTokenType(builder);
            if (tt == SpringTokenType.Begin)
            {
                ParseCompoundStatement(builder);
            }
            else if (tt == SpringTokenType.ProcedureCall)
            {
                ParseProcedureCall(builder);
            }
            else if (tt == SpringTokenType.Identifier)
            {
                ParseAssignStatement(builder);
            }
            else
            {
                builder.Error("Expected statement");
            }

            ExpectToken(builder, SpringTokenType.Semi);
            builder.TryAdvance();
        }

        private void ParseProcedureCall(PsiBuilder builder)
        {
            var start = builder.Mark();
            builder.TryAdvance();
            ExpectToken(builder, SpringTokenType.LeftParenthesis);
            builder.TryAdvance();
            ParseExpr(builder);
            if (ExpectToken(builder, SpringTokenType.RightParenthesis, start))
            {
                builder.Done(start, SpringCompositeNodeType.Statement, null);
            }

            builder.TryAdvance();
        }

        private void ParseAssignStatement(PsiBuilder builder)
        {
            var start = builder.Mark();
            var identToken = builder.TryGetToken();
            builder.TryAdvance();
            ExpectToken(builder, SpringTokenType.Assignment);
            var assignToken = builder.TryGetToken();
            builder.TryAdvance();
            ParseExpr(builder);
            var statement = new AssignStatementNode(new VariableNode(identToken), assignToken);
            builder.Done(start, SpringCompositeNodeType.Statement, statement);
        }

        private void ParseExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseTerm(builder);
            BinOpNode left = null;

            var tt = GetTokenType(builder);

            while (tt == SpringTokenType.Plus
                   || tt == SpringTokenType.Minus)
            {
                left = new BinOpNode(builder.TryGetToken());
                builder.TryAdvance();
                ParseTerm(builder);
                tt = GetTokenType(builder);
            }

            builder.Done(start, SpringCompositeNodeType.Expression, left);
        }

        private void ParseTerm(PsiBuilder builder)
        {
            var start = builder.Mark();
            var tt = GetTokenType(builder);
            if (tt == SpringTokenType.String)
            {
                builder.Done(start, SpringCompositeNodeType.Expression, null);
                builder.TryAdvance();
                return;
            }

            ParseFactor(builder);
            tt = GetTokenType(builder);
            BinOpNode left = null;

            while (tt == SpringTokenType.Multiply
                   || tt == SpringTokenType.Divide)
            {
                left = new BinOpNode(builder.TryGetToken());
                builder.TryAdvance();
                ParseFactor(builder);
            }

            builder.Done(start, SpringCompositeNodeType.Expression, left);
        }

        private void ParseFactor(PsiBuilder builder)
        {
            var tt = GetTokenType(builder);

            if (tt == SpringTokenType.Plus
                || tt == SpringTokenType.Minus)
            {
                ParseUnaryExpr(builder);
            }
            else if (tt == SpringTokenType.LeftParenthesis)
            {
                ParseBracketExpr(builder);
            }
            else if (tt == SpringTokenType.Number || tt == SpringTokenType.Identifier)
                ParseLiteral(builder);
            else
            {
                builder.Error("Expected factor");
            }
        }

        private void ParseLiteral(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.Number || tt == SpringTokenType.Identifier)
            {
                builder.TryAdvance();
            }
            else
            {
                builder.Error("Expected literal");
            }
        }

        private void ParseUnaryExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            var tt = GetTokenType(builder);


            if (tt == SpringTokenType.Plus
                || tt == SpringTokenType.Minus)
            {
                ParseUnaryExpr(builder);
            }
            else
            {
                ParseFactor(builder);
            }

            builder.TryAdvance();
            builder.Done(start, SpringCompositeNodeType.UnaryOp, new UnaryOpNode(builder.TryGetToken()));
        }

        private void ParseBracketExpr(PsiBuilder builder)
        {
            builder.TryAdvance();
            ParseExpr(builder);
            ExpectToken(builder, SpringTokenType.RightParenthesis);
            builder.TryAdvance();
        }

        public class TokenFactory : IPsiBuilderTokenFactory
        {
            public LeafElementBase CreateToken(TokenNodeType tokenNodeType, IBuffer buffer, int startOffset,
                int endOffset)
            {
                return tokenNodeType.Create(buffer, new TreeOffset(startOffset), new TreeOffset(endOffset));
            }
        }
    }

    public static class TokenNodeTypeExtensions
    {
        public static bool TryAdvance(this PsiBuilder builder)
        {
            if (builder.Eof())
            {
                return false;
            }

            builder.AdvanceLexer();
            while (builder.GetTokenType() == SpringTokenType.Whitespace || builder.GetTokenType() == SpringTokenType.Comment)
            {
                builder.AdvanceLexer();
                if (builder.Eof())
                    return false;
            }
            return true;
        }

        public static Token TryGetToken(this PsiBuilder builder)
        {
            return builder.Eof() ? new Token() : builder.GetToken();
        }
    }
}
