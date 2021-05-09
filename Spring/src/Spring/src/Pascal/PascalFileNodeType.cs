using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    internal class PascalFileNodeType : CompositeNodeType
    {
        public PascalFileNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly PascalFileNodeType Instance = new("File", 0);
        public static readonly PascalFileNodeType Variable = new("Variable", 1);
        public static readonly PascalFileNodeType Num = new("Num", 2);

        public override CompositeElement Create()
        {
            return new PascalFile();
        }
    }

    internal class PascalCompositeNodeType : CompositeNodeType
    {
        public PascalCompositeNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly PascalCompositeNodeType CompoundStatement = new("CompoundStatement", 0);
        public static readonly PascalCompositeNodeType AssignmentStatement = new("AssignmentStatement", 1);
        public static readonly PascalCompositeNodeType UnaryOp = new("UnaryOp", 2);
        public static readonly PascalCompositeNodeType BinOp = new("BinOp", 3);
        public static readonly PascalCompositeNodeType Expression = new("Expression", 4);
        public static readonly PascalCompositeNodeType Instance = new("Instance", 5);

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
