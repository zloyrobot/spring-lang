using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class SpringFileNodeType : CompositeNodeType
    {
        public SpringFileNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringFileNodeType Instance = new SpringFileNodeType("Spring_FILE", 0);

        public override CompositeElement Create()
        {
            return new SpringFile();
        }
    }
    internal class SpringCompositeNodeType : CompositeNodeType
    {
        public SpringCompositeNodeType(string s, int index) : base(s, index)
        {
        }
        public static readonly SpringCompositeNodeType SEQ = new SpringCompositeNodeType("Spring_SEQ", 0);
        public static readonly SpringCompositeNodeType ASSIGN = new SpringCompositeNodeType("Spring_ASSIGN", 1);
        public static readonly SpringCompositeNodeType LOW_BINOP = new SpringCompositeNodeType("Spring_LOW_BINOP", 2);
        public static readonly SpringCompositeNodeType MEDIUM_BINOP = new SpringCompositeNodeType("Spring_MEDIUM_BINOP", 3);
        public static readonly SpringCompositeNodeType HIGH_BINOP = new SpringCompositeNodeType("Spring_HIGH_BINOP", 4);
        public static readonly SpringCompositeNodeType NUMBER = new SpringCompositeNodeType("Spring_NUMBER", 5);
        public static readonly SpringCompositeNodeType VARIABLE = new SpringCompositeNodeType("Spring_VARIABLE", 6);
        public static readonly SpringCompositeNodeType STRING = new SpringCompositeNodeType("Spring_STRING", 7);
        public static readonly SpringCompositeNodeType FOR = new SpringCompositeNodeType("Spring_FOR", 8);
        public static readonly SpringCompositeNodeType LOGIC_BINOP = new SpringCompositeNodeType("Spring_LOGIC_BINOP", 9);

        public override CompositeElement Create()
        {
            if (this == SEQ)
                return new SpringSeq();
            if (this == ASSIGN)
                return new SpringAssign();
            if (this == LOW_BINOP)
                return new SpringLowBinop();
            if (this == MEDIUM_BINOP)
                return new SpringMediumBinop();
            if (this == HIGH_BINOP)
                return new SpringHighBinop();
            if (this == NUMBER)
                return new SpringNumber();
            if (this == VARIABLE)
                return new SpringVariable();
            if (this == STRING)
                return new SpringString();
            if (this == FOR)
                return new SpringFor();
            if (this == LOGIC_BINOP)
                return new SpringLogicBinop();
            throw new InvalidOperationException();
        }
    }

}