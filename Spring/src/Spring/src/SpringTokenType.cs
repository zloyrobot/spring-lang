using System;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.JavaScript.Impl.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringTokenType : TokenNodeType
    {
        public static SpringTokenType WHITE_SPACE = new SpringTokenType("WHITE_SPACE", 0);
        public static SpringTokenType ASSIGN = new SpringTokenType("ASSIGN", 1);
        public static SpringTokenType LOW_BINOP = new SpringTokenType("LOW_BINOP", 2);
        public static SpringTokenType MEDIUM_BINOP = new SpringTokenType("MEDIUM_BINOP", 3);
        public static SpringTokenType HIGH_BINOP = new SpringTokenType("HIGH_BINOP", 4);
        public static SpringTokenType IDENT = new SpringTokenType("IDENT", 5);
        public static SpringTokenType LBRACKET = new SpringTokenType("LBRACKET", 6);
        public static SpringTokenType RBRACKET = new SpringTokenType("RBRACKET", 7);
        public static SpringTokenType SEQ = new SpringTokenType("SEQ", 8);
        public static SpringTokenType NUMBER = new SpringTokenType("NUMBER", 9);
        public static SpringTokenType STRING = new SpringTokenType("STRING", 10);
        public static SpringTokenType BAD_CHARACTER = new SpringTokenType("BAD_CHARACTER", 11);
        public static SpringTokenType LFBRACKET = new SpringTokenType("LFBRACKET", 12);
        public static SpringTokenType RFBRACKET = new SpringTokenType("RFBRACKET", 13);
        public static SpringTokenType LOGIC_BINOP = new SpringTokenType("LOGIC_BINOP", 14);
        public static SpringTokenType FOR = new SpringTokenType("FOR", 15);
        public static SpringTokenType READ = new SpringTokenType("FOR", 16);
        public static SpringTokenType WRITE = new SpringTokenType("FOR", 17);

        public SpringTokenType(string s, int index) : base(s, index)
        {
            TokenRepresentation = s;
        }

        public override LeafElementBase Create(IBuffer buffer,
            TreeOffset startOffset,
            TreeOffset endOffset)
        {
            return new SpringGenericToken(this, buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)));
        }

        public override bool IsWhitespace => this == WHITE_SPACE;
        public override bool IsComment => false;
        public override bool IsStringLiteral => this == STRING;
        public override bool IsConstantLiteral => this == NUMBER;
        
        public override bool IsIdentifier => this == IDENT;
        public override bool IsKeyword => this == FOR || this == READ || this == WRITE;
        public override string TokenRepresentation { get; }
    }
}