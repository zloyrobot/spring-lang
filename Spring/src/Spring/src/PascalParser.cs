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
        private static void ExpectToken(TreeBuilder builder, SpringTokenType tokenType)
        {
            var tt = builder.GetTokenType();
            if (tt != tokenType)
                builder.Error($"Invalid syntax. Expected {tokenType} but {tt} is given.");
        }

        private void ParseCompoundStatement(TreeBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.Begin)
            {
                var start = builder.Mark();
                builder.AdvanceLexer();
                ParseStatementList(builder);

                if (builder.GetTokenType() != SpringTokenType.End)
                    builder.Error("Expected 'END'");
                else
                    builder.AdvanceLexer();

                builder.Done(start, SpringCompositeNodeType.CompoundStatement, null);
            }
            else if (tt == SpringTokenType.End)
                return;
            else builder.AdvanceLexer();
        }

        private void ParseStatementList(TreeBuilder builder)
        {
            var tt = builder.GetTokenType();
            ParseStatement(builder);

            while (tt == SpringTokenType.Semi)
            {
                builder.AdvanceLexer();
                ParseStatement(builder);
                tt = builder.GetTokenType();
            }

            builder.AdvanceLexer();
        }

        private void ParseStatement(TreeBuilder builder)
        {
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.Begin)
            {
                ParseCompoundStatement(builder);
            }
            else if (tt == SpringTokenType.Variable)
            {
                ParseAssignStatement(builder);
            }
        }

        private void ParseAssignStatement(TreeBuilder builder)
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

        private void ParseExpr(TreeBuilder builder)
        {
            var start = builder.Mark();
            ParseTerm(builder);
            BinOpNode left = null;

            var tt = builder.GetTokenType();

            while (tt == SpringTokenType.Plus
                   || tt == SpringTokenType.Minus)
            {
                left = new BinOpNode(builder.GetToken());
                builder.AdvanceLexer();
                ParseTerm(builder);
            }

            builder.Done(start, SpringCompositeNodeType.Expression, left);
        }

        private void ParseTerm(TreeBuilder builder)
        {
            var start = builder.Mark();
            ParseFactor(builder);
            var tt = builder.GetTokenType();
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

        private void ParseFactor(TreeBuilder builder)
        {
            var start = builder.Mark();

            var tt = builder.GetTokenType();

            if (tt == SpringTokenType.Plus
                || tt == SpringTokenType.Minus)
            {
                ParseUnaryExpr(builder);
                return;
            }

            if (tt == SpringTokenType.LeftParenthesis)
            {
                ParseBracketExpr(builder);
            }
            if (tt == SpringTokenType.Variable)
            {
                ParseVariable(builder);
            }
            ExpectToken(builder, SpringTokenType.Number);
            builder.AdvanceLexer();
            builder.Done(start, SpringFileNodeType.Num, new NumNode(builder.GetToken()));
        }

        private void ParseVariable(TreeBuilder builder)
        {
            var start = builder.Mark();
            builder.Done(start, SpringFileNodeType.Num, new VariableNode(builder.GetToken()));
            builder.AdvanceLexer();
        }

        private void ParseUnaryExpr(TreeBuilder builder)
        {
            var start = builder.Mark();
            var tt = builder.GetTokenType();


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

        private void ParseBracketExpr(TreeBuilder builder)
        {
            builder.AdvanceLexer();
            ParseExpr(builder);
            ExpectToken(builder, SpringTokenType.RightParenthesis);
            builder.AdvanceLexer();
        }

        public IFile ParseFile()
        {
            using var def = Lifetime.Define();
            var builder = new TreeBuilder(_myLexer, SpringFileNodeType.Instance, new TokenFactory(), def.Lifetime);
            var fileMark = builder.Mark();
            _myLexer.Start();
            builder.AdvanceLexer();

            ParseCompoundStatement(builder);

            builder.Done(fileMark, SpringFileNodeType.Instance, null);
            var file = (IFile) builder.BuildTree();
            return file;
        }
        
        public class TokenFactory : IPsiBuilderTokenFactory
        {
            public LeafElementBase CreateToken(TokenNodeType tokenNodeType, IBuffer buffer, int startOffset, int endOffset)
            {
                return tokenNodeType.Create(buffer, new TreeOffset(startOffset), new TreeOffset(endOffset));
            }
        }
    }
}
