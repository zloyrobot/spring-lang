using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring.Spring
{
    class SpringTokenType : TokenNodeType
    {
        public static  SpringTokenType EQ = new SpringTokenType("EQ", 0);
        public static  SpringTokenType NUMBER = new SpringTokenType("NUMBER", 1);
        public static SpringTokenType STRING = new SpringTokenType("STRING", 2);
        public static SpringTokenType BAD_CHARACTER = new SpringTokenType("BAD_CHARACTER", 3);
        public SpringTokenType(string s, int index) : base(s, index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsWhitespace { get; }
        public override bool IsComment { get; }
        public override bool IsStringLiteral { get; }
        public override bool IsConstantLiteral { get; }
        public override bool IsIdentifier { get; }
        public override bool IsKeyword { get; }
        public override string TokenRepresentation { get; }
    }
}