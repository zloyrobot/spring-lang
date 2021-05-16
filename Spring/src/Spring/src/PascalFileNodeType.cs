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

        public override CompositeElement Create()
        {
            return new SpringFile();
        }
    }

    public class SpringLiteralType : NodeType
    {
        public static readonly SpringLiteralType Literal = new("Literal", 0);
        public SpringLiteralType(string s, int index) : base(s, index)
        {
        }
        
    }

    public class SpringCompositeNodeType : CompositeNodeType
    {
        public SpringCompositeNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringCompositeNodeType Statement = new("Statement", 0);
        public static readonly SpringCompositeNodeType UnaryOp = new("UnaryOp", 2);
        public static readonly SpringCompositeNodeType BinOp = new("BinOp", 3);
        public static readonly SpringCompositeNodeType Expression = new("Expression", 4);

        public override CompositeElement Create()
        {
            if (this == Statement)
                return new CompoundStatement();
            if (this == UnaryOp)
                return new UnaryOp();
            if (this == BinOp)
                return new BinOp();
            if (this == Expression)
                return new Expression();

            throw new InvalidOperationException();
        }
    }
}
