using JetBrains.ReSharper.Psi.Parsing;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class AssignStatementNode : Node
    {
        private readonly VariableNode _variable;
        private readonly Token _assignToken;

        public AssignStatementNode(VariableNode variable, Token assignToken)
        {
            _variable = variable;
            _assignToken = assignToken;
        }
    }

    internal abstract class Node
    {
    }

    internal class NumNode : Node
    {
        private Token Token { get; set; }

        public NumNode(Token SpringToken)
        {
            Token = SpringToken;
        }
    }

    internal class VariableNode : Node
    {
        private Token Token { get; }

        public VariableNode(Token SpringToken)
        {
            Token = SpringToken;
        }
    }

    internal class UnaryOpNode : Node
    {
        private Token Op { get; set; }

        public UnaryOpNode(Token op)
        {
            Op = op;
        }
    }

    internal class BinOpNode : Node
    {
        private Token Op { get; set; }
        // private Node Left { get; set; }
        // private Node Right { get; set; }

        public BinOpNode(Token op)
        {
            Op = op;
            // Left = left;
            // Right = right;
        }
    }
}
