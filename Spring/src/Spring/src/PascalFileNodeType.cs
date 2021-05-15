using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFileNodeType : CompositeNodeType
    {
        public SpringFileNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringFileNodeType Instance = new("File", 0);
        public static readonly SpringFileNodeType Literal = new("Literal", 1);

        public override CompositeElement Create()
        {
            return new SpringFile();
        }
    }

    public class SpringCompositeNodeType : CompositeNodeType
    {
        public SpringCompositeNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringCompositeNodeType CompoundStatement = new("CompoundStatement", 0);
        public static readonly SpringCompositeNodeType AssignmentStatement = new("AssignmentStatement", 1);
        public static readonly SpringCompositeNodeType UnaryOp = new("UnaryOp", 2);
        public static readonly SpringCompositeNodeType BinOp = new("BinOp", 3);
        public static readonly SpringCompositeNodeType Expression = new("Expression", 4);
        public static readonly SpringCompositeNodeType Instance = new("Instance", 5);

        public override CompositeElement Create()
        {
            if (this == CompoundStatement)
                return new CompoundStatement();
            if (this == AssignmentStatement)
                return new AssignmentStatement();
            if (this == UnaryOp)
                return new UnaryOp();
            if (this == BinOp)
                return new BinOp();

            throw new InvalidOperationException();
        }
    }
}
