using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    class PascalTokenType : TokenNodeType
    {
        public static readonly PascalTokenType None = new("NONE", 0);
        public static readonly PascalTokenType Number = new("NUMBER", 1);
        public static readonly PascalTokenType String = new("STRING", 2);
        public static readonly PascalTokenType Comment = new("COMMENT", 3);
        public static readonly PascalTokenType Identifier = new("IDENTIFIER", 4);
        public static readonly PascalTokenType Plus = new("PLUS", 5);
        public static readonly PascalTokenType Minus = new("MINUS", 6);
        public static readonly PascalTokenType Multiply = new("DIVIDE", 17);
        public static readonly PascalTokenType Divide = new("DIVIDE", 7);
        public static readonly PascalTokenType Variable = new("VARIABLE", 8);
        public static readonly PascalTokenType LeftParenthesis = new("LEFT_PARENTHESIS", 9);
        public static readonly PascalTokenType RightParenthesis = new("RIGHT_PARENTHESIS", 10);
        public static readonly PascalTokenType Assignment = new("ASSIGNMENT", 11);
        public static readonly PascalTokenType End = new("END", 12);
        public static readonly PascalTokenType Begin = new("BEGIN", 13);
        public static readonly PascalTokenType Dot = new("DOT", 14);
        public static readonly PascalTokenType Semi = new("SEMI", 15);
        public static readonly PascalTokenType BadCharacter = new("BAD_CHARACTER", 16);

        private string Text;

        public PascalTokenType(string s, int index) : base(s, index)
        {
            Text = s;
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new PascalToken(this, buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override bool IsWhitespace => false;
        public override bool IsComment => this == Comment;
        public override bool IsStringLiteral => this == String;
        public override bool IsConstantLiteral => this == Number;
        public override bool IsIdentifier => this == Identifier;

        public override bool IsKeyword => new[]
        {
            "and", "begin", "boolean", "break", "byte", "continue", "div", "do", "double", "else", "end", "false", "if",
            "integer", "longint", "mod", "not", "or", "repeat", "shl", "shortint", "shr", "single", "then", "true",
            "until", "while", "word", "xor"
        }.Contains(TokenRepresentation);

        public override string TokenRepresentation => Text;
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
            return new StringBuffer(_text);
        }

        public override string GetText()
        {
            return _text;
        }

        public override NodeType NodeType { get; }
        public override PsiLanguageType Language => PascalLanguage.Instance;

        public TokenNodeType GetTokenType()
        {
            return (TokenNodeType) NodeType;
        }
    }
}
