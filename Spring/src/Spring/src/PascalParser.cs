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
            var tt = getTokenType(builder);
            if (tt != tokenType)
                builder.Error($"Invalid syntax. Expected {tokenType} but {tt} is given.");
        }

        private static TokenNodeType getTokenType(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (!tt.IsWhitespace) return tt;
            builder.AdvanceLexer();
            tt = builder.GetTokenType();
            return tt;
        }

        private void ParseCompoundStatement(PsiBuilder builder)
        {
            var tt = getTokenType(builder);
            if (tt == SpringTokenType.Begin)
            {
                var start = builder.Mark();
                builder.AdvanceLexer();
                ParseStatementList(builder);

                if (getTokenType(builder) != SpringTokenType.End)
                    builder.Error("Expected 'END'");
                else
                    builder.AdvanceLexer();

                builder.Done(start, SpringCompositeNodeType.CompoundStatement, null);
            }
            else if (tt == SpringTokenType.End)
                return;

            // else builder.AdvanceLexer();
        }

        private void ParseStatementList(PsiBuilder builder)
        {
            var tt = getTokenType(builder);
            ParseStatement(builder);

            while (tt == SpringTokenType.Semi)
            {
                builder.AdvanceLexer();
                ParseStatement(builder);
                tt = getTokenType(builder);
            }

            builder.AdvanceLexer();
        }

        private void ParseStatement(PsiBuilder builder)
        {
            var tt = getTokenType(builder);
            if (tt == SpringTokenType.Begin)
            {
                ParseCompoundStatement(builder);
            }
            else if (tt == SpringTokenType.ProcedureCall)
            {
                ParseProcedureCall(builder);
            }
            else if (tt == SpringTokenType.Variable)
            {
                ParseAssignStatement(builder);
            }
        }

        private void ParseProcedureCall(PsiBuilder builder)
        {
        }

        private void ParseAssignStatement(PsiBuilder builder)
        {
            // var varToken = EatToken(SpringTokenType.Variable);
            var start = builder.Mark();
            var varToken = builder.GetToken();
            builder.AdvanceLexer();
            ExpectToken(builder, SpringTokenType.Assignment);
            var assignToken = builder.GetToken();
            builder.AdvanceLexer();
            ParseExpr(builder);
            var statement = new AssignStatementNode(new VariableNode(varToken), assignToken);
            builder.Done(start, SpringCompositeNodeType.AssignmentStatement, statement);
        }

        private void ParseExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseTerm(builder);
            BinOpNode left = null;

            var tt = getTokenType(builder);
            if (tt == SpringTokenType.String)
            {
                var st = builder.Mark();
                builder.Done(st, SpringFileNodeType.Literal, new VariableNode(builder.GetToken()));
                builder.Done(start, SpringCompositeNodeType.Expression, null);
                return;
            }

            while (tt == SpringTokenType.Plus
                   || tt == SpringTokenType.Minus)
            {
                left = new BinOpNode(builder.GetToken());
                builder.AdvanceLexer();
                ParseTerm(builder);
            }

            builder.Done(start, SpringCompositeNodeType.Expression, left);
        }

        private void ParseTerm(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseFactor(builder);
            var tt = getTokenType(builder);
            BinOpNode left = null;

            while (tt == SpringTokenType.Multiply
                   || tt == SpringTokenType.Divide)
            {
                left = new BinOpNode(builder.GetToken());
                builder.AdvanceLexer();
                ParseFactor(builder);
            }

            builder.Done(start, SpringCompositeNodeType.Expression, left);
        }

        private void ParseFactor(PsiBuilder builder)
        {
            var start = builder.Mark();

            var tt = getTokenType(builder);

            if (tt == SpringTokenType.Plus
                || tt == SpringTokenType.Minus)
            {
                ParseUnaryExpr(builder);
            }

            if (tt == SpringTokenType.LeftParenthesis)
            {
                ParseBracketExpr(builder);
            }

            if (tt == SpringTokenType.Variable)
            {
                ParseVariable(builder);
            }

            // ExpectToken(builder, SpringTokenType.NUMBER);
            // builder.AdvanceLexer();
            builder.Done(start, SpringFileNodeType.Literal, new NumNode(builder.GetToken()));
        }

        private void ParseVariable(PsiBuilder builder)
        {
            var start = builder.Mark();
            builder.Done(start, SpringFileNodeType.Literal, new VariableNode(builder.GetToken()));
            builder.AdvanceLexer();
        }

        private void ParseUnaryExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            var tt = getTokenType(builder);


            if (tt == SpringTokenType.Plus
                || tt == SpringTokenType.Minus)
            {
                ParseUnaryExpr(builder);
            }
            else
            {
                ParseFactor(builder);
            }

            builder.Done(start, SpringCompositeNodeType.UnaryOp, new UnaryOpNode(builder.GetToken()));
            builder.AdvanceLexer();
        }

        private void ParseBracketExpr(PsiBuilder builder)
        {
            builder.AdvanceLexer();
            ParseExpr(builder);
            ExpectToken(builder, SpringTokenType.RightParenthesis);
            builder.AdvanceLexer();
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
}
