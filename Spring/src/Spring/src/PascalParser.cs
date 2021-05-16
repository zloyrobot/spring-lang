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

        private static void ExpectToken(PsiBuilder builder, SpringTokenType tokenType)
        {
            var tt = GetTokenType(builder);
            if (tt != tokenType)
                builder.Error($"Invalid syntax. Expected {tokenType} but {tt} is given.");
        }

        private static TokenNodeType GetTokenType(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == null)
            {
                return null;
            }

            if (!tt.IsWhitespace) return tt;
            builder.TryAdvance();
            tt = builder.GetTokenType();
            return tt;
        }

        private void ParseCompoundStatement(PsiBuilder builder)
        {
            var tt = GetTokenType(builder);
            if (tt == SpringTokenType.Begin)
            {
                // var start = builder.Mark();
                builder.TryAdvance();
                // ParseStatementList(builder);
                //
                // if (GetTokenType(builder) != SpringTokenType.End)
                //     builder.Error("Expected 'END'");
                // else
                //     builder.TryAdvance();
                //
                // builder.Done(start, SpringCompositeNodeType.CompoundStatement, null);
                if (GetTokenType(builder) != SpringTokenType.End)
                {
                    builder.Error("fuck you");
                }
                
            }
            else if (tt == SpringTokenType.End)
                return;

            else builder.TryAdvance();
        }

        private void ParseStatementList(PsiBuilder builder)
        {
            var tt = GetTokenType(builder);
            ParseStatement(builder);

            while (tt == SpringTokenType.Semi)
            {
                builder.TryAdvance();
                ParseStatement(builder);
                tt = GetTokenType(builder);
            }

            builder.TryAdvance();
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
        }

        private void ParseProcedureCall(PsiBuilder builder)
        {
            var start = builder.Mark();
            var lbr = builder.TryGetToken();
            ExpectToken(builder, SpringTokenType.LeftParenthesis);
            builder.TryAdvance();
            ParseExpr(builder);
            ExpectToken(builder, SpringTokenType.RightParenthesis);
            builder.TryAdvance();
            builder.Done(start, SpringCompositeNodeType.AssignmentStatement, null);
        }

        private void ParseAssignStatement(PsiBuilder builder)
        {
            var start = builder.Mark();
            var varToken = builder.TryGetToken();
            builder.TryAdvance();
            ExpectToken(builder, SpringTokenType.Assignment);
            var assignToken = builder.TryGetToken();
            builder.TryAdvance();
            ParseExpr(builder);
            var statement = new AssignStatementNode(new VariableNode(varToken), assignToken);
            builder.Done(start, SpringCompositeNodeType.AssignmentStatement, statement);
        }

        private void ParseExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseTerm(builder);
            BinOpNode left = null;

            var tt = GetTokenType(builder);
            if (tt == SpringTokenType.String)
            {
                // var st = builder.Mark();
                // builder.Precede();
                // builder.Done(st, SpringLiteralType.Literal, new VariableNode(builder.TryGetToken()));
                builder.Done(start, SpringCompositeNodeType.Expression, null);
                return;
            }

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
            ParseFactor(builder);
            var tt = GetTokenType(builder);
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
            else if (tt == SpringTokenType.Identifier)
            {
                ParseVariable(builder);
            }
        }

        private void ParseVariable(PsiBuilder builder)
        {
            // var start = builder.Mark();
            // builder.Done(start, SpringLiteralType.Literal, new VariableNode(builder.TryGetToken()));
            builder.TryAdvance();
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

            if (builder.TryAdvance())
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
                builder.Error("Unexpected end of file");
                return false;
            }

            builder.AdvanceLexer();
            return true;
        }

        public static Token TryGetToken(this PsiBuilder builder)
        {
            if (builder.Eof())
            {
                builder.Error("Unexpected end of file");
                return new Token();
            }

            return builder.GetToken();
        }
    }
}
