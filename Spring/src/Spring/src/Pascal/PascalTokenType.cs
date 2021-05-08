using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    class PascalTokenType : TokenNodeType
    {
        public static  PascalTokenType Symbol = new PascalTokenType("SYMBOL", 0);
        public static  PascalTokenType Number = new PascalTokenType("NUMBER", 1);
        public static PascalTokenType String = new PascalTokenType("STRING", 2);
        public static PascalTokenType Comment = new PascalTokenType("COMMENT", 3);
        public static PascalTokenType Identifier = new PascalTokenType("IDENTIFIER", 4);
        public static PascalTokenType BadCharacter = new PascalTokenType("BAD_CHARACTER", 5);
        public PascalTokenType(string s, int index) : base(s, index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsWhitespace => false;
        public override bool IsComment => this == Comment;
        public override bool IsStringLiteral => this == String;
        public override bool IsConstantLiteral { get; }
        public override bool IsIdentifier => this == Identifier;
        public override bool IsKeyword => false; // TODO
        public override string TokenRepresentation { get; }
    }

    public class PascalToken : LeafElementBase, ITokenNode
    {
        private readonly string _text;

        public PascalToken(NodeType nodeType, string text)
        {
            NodeType = nodeType;
            _text = text;
        }
        public override int GetTextLength()
        {
            return _text.Length;
        }

        public override StringBuilder GetText(StringBuilder to)
        {
            return to.Append(_text);
        }

        public override IBuffer GetTextAsBuffer()
        {
            return StringBuffer(_text);
        }

        public override string GetText()
        {
            return _text;
        }

        public override NodeType NodeType { get; }
        public override PsiLanguageType Language { get; }
        public TokenNodeType GetTokenType()
        {
            throw new System.NotImplementedException();
        }
    }
}
