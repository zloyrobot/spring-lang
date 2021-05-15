using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring
{
    class SpringTokenType : TokenNodeType
    {
        public static readonly SpringTokenType None = new("NONE", 0);
        public static readonly SpringTokenType NUMBER = new("NUMBER", 1);
        public static readonly SpringTokenType String = new("STRING", 2);
        public static readonly SpringTokenType Comment = new("COMMENT", 3);
        public static readonly SpringTokenType Identifier = new("IDENTIFIER", 4);
        public static readonly SpringTokenType Plus = new("PLUS", 5);
        public static readonly SpringTokenType Minus = new("MINUS", 6);
        public static readonly SpringTokenType Multiply = new("DIVIDE", 17);
        public static readonly SpringTokenType Divide = new("DIVIDE", 7);
        public static readonly SpringTokenType ControlSequence = new("CONTROL_SEQUENCE", 8);
        public static readonly SpringTokenType LeftParenthesis = new("LEFT_PARENTHESIS", 9);
        public static readonly SpringTokenType RightParenthesis = new("RIGHT_PARENTHESIS", 10);
        public static readonly SpringTokenType Assignment = new("ASSIGNMENT", 11);
        public static readonly SpringTokenType End = new("END", 12);
        public static readonly SpringTokenType Begin = new("BEGIN", 13);
        public static readonly SpringTokenType Dot = new("DOT", 14);
        public static readonly SpringTokenType Semi = new("SEMI", 15);
        public static readonly SpringTokenType BAD_CHARACTER = new("BAD_CHARACTER", 16);
        public static readonly SpringTokenType EQ = new("EQ", 17);
        public static readonly SpringTokenType Whitespace = new("Whitespace", 18);
        public static readonly SpringTokenType ProcedureCall = new("ProcedureCall", 19);

        public SpringTokenType(string s, int index) : base(s, index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new SpringToken(this, buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override bool IsWhitespace => this == Whitespace;
        public override bool IsComment => this == Comment;
        public override bool IsStringLiteral => this == String;
        public override bool IsConstantLiteral => this == NUMBER;
        public override bool IsIdentifier => this == Identifier;

        public override bool IsKeyword => this == Begin || this == End || this == ControlSequence;

        public override string TokenRepresentation => ToString();
    }

    public class SpringToken : LeafElementBase, ITokenNode
    {
        private readonly string _text;

        public SpringToken(NodeType nodeType, string text)
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
            return new StringBuffer(_text);
        }

        public override string GetText()
        {
            return _text;
        }

        public override NodeType NodeType { get; }
        public override PsiLanguageType Language => SpringLanguage.Instance;

        public TokenNodeType GetTokenType()
        {
            return (TokenNodeType) NodeType;
        }

        public override bool Equals(object obj)
        {
            var nt = obj as SpringToken;
            if (nt == null)
            {
                return false;
            }

            return GetTokenType() == nt.GetTokenType() && GetText().Equals(nt.GetText());
        }

        public override string ToString()
        {
            return GetText();
        }
    }
}
