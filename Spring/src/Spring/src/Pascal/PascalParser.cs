using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.TreeBuilder;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    internal class PascalParser : IParser
    {
        private readonly ILexer _myLexer;

        public PascalParser(ILexer lexer)
        {
            _myLexer = lexer;
        }
        private static void ExpectToken(PsiBuilder builder, PascalTokenType tokenType)
        {
            var tt = builder.GetTokenType();
            if (tt != tokenType)
                builder.Error($"Invalid syntax. Expected {tokenType} but {tt} is given.");
        }

        private void ParseCompoundStatement(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == PascalTokenType.Begin)
            {
                var start = builder.Mark();
                builder.AdvanceLexer();
                ParseStatementList(builder);

                if (builder.GetTokenType() != PascalTokenType.End)
                    builder.Error("Expected 'END'");
                else
                    builder.AdvanceLexer();

                builder.Done(start, PascalCompositeNodeType.CompoundStatement, null);
            }
            else if (tt == PascalTokenType.End)
                return;
            else builder.AdvanceLexer();
        }

        private void ParseStatementList(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            ParseStatement(builder);

            while (tt == PascalTokenType.Semi)
            {
                builder.AdvanceLexer();
                ParseStatement(builder);
                tt = builder.GetTokenType();
            }

            builder.AdvanceLexer();
        }

        private void ParseStatement(PsiBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == PascalTokenType.Begin)
            {
                ParseCompoundStatement(builder);
            }
            else if (tt == PascalTokenType.Variable)
            {
                ParseAssignStatement(builder);
            }
        }

        private void ParseAssignStatement(PsiBuilder builder)
        {
            // var varToken = EatToken(PascalTokenType.Variable);
            var start = builder.Mark();
            var varToken = builder.GetToken();
            builder.AdvanceLexer();
            ExpectToken(builder, PascalTokenType.Assignment);
            var assignToken = builder.GetToken();
            builder.AdvanceLexer();
            ParseExpr(builder);
            var statement = new AssignStatementNode(new VariableNode(varToken), assignToken);
            builder.Done(start, PascalCompositeNodeType.AssignmentStatement, statement);
        }

        private void ParseExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseTerm(builder);
            BinOpNode left = null;

            var tt = builder.GetTokenType();

            while (tt == PascalTokenType.Plus
                   || tt == PascalTokenType.Minus)
            {
                left = new BinOpNode(builder.GetToken());
                builder.AdvanceLexer();
                ParseTerm(builder);
            }

            builder.Done(start, PascalCompositeNodeType.Expression, left);
        }

        private void ParseTerm(PsiBuilder builder)
        {
            var start = builder.Mark();
            ParseFactor(builder);
            var tt = builder.GetTokenType();
            BinOpNode left = null;

            while (tt == PascalTokenType.Multiply
                   || tt == PascalTokenType.Divide)
            {
                left = new BinOpNode(builder.GetToken());
                builder.AdvanceLexer();
                ParseFactor(builder);
            }

            builder.Done(start, PascalCompositeNodeType.Expression, left);
        }

        private void ParseFactor(PsiBuilder builder)
        {
            var start = builder.Mark();

            var tt = builder.GetTokenType();

            if (tt == PascalTokenType.Plus
                || tt == PascalTokenType.Minus)
            {
                ParseUnaryExpr(builder);
                return;
            }

            if (tt == PascalTokenType.LeftParenthesis)
            {
                ParseBracketExpr(builder);
            }
            if (tt == PascalTokenType.Variable)
            {
                ParseVariable(builder);
            }
            ExpectToken(builder, PascalTokenType.Number);
            builder.AdvanceLexer();
            builder.Done(start, PascalFileNodeType.Num, new NumNode(builder.GetToken()));
        }

        private void ParseVariable(PsiBuilder builder)
        {
            var start = builder.Mark();
            builder.Done(start, PascalFileNodeType.Num, new VariableNode(builder.GetToken()));
            builder.AdvanceLexer();
        }

        private void ParseUnaryExpr(PsiBuilder builder)
        {
            var start = builder.Mark();
            var tt = builder.GetTokenType();


            if (tt == PascalTokenType.Plus
                || tt == PascalTokenType.Minus)
            {
                ParseUnaryExpr(builder);
            }
            else
            {
                ParseFactor(builder);
            }
            builder.Done(start, PascalCompositeNodeType.UnaryOp, new UnaryOpNode(builder.GetToken()));
            builder.AdvanceLexer();
        }

        private void ParseBracketExpr(PsiBuilder builder)
        {
            builder.AdvanceLexer();
            ParseExpr(builder);
            ExpectToken(builder, PascalTokenType.RightParenthesis);
            builder.AdvanceLexer();
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
    }
}
