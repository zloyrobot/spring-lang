using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.JavaScript.Impl.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Impl.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring
{
    class SpringTokenType : TokenNodeType
    {
        public static SpringTokenType WHITE_SPACE_SECTION = new SpringTokenType("WHITE_SPACE_SECTION", 0);
        public static SpringTokenType COMMENT_SECTION = new SpringTokenType("COMMENT_SECTION", 1);
        public static SpringTokenType UINT_NUMBER = new SpringTokenType("UINT_NUMBER", 2);
        public static SpringTokenType NON_UINT_NUMBER = new SpringTokenType("NON_UINT_NUMBER", 3);
        public static SpringTokenType LOGICAL_CONST = new SpringTokenType("LOGICAL_CONST", 4);
        public static SpringTokenType ROUND_LBRACE = new SpringTokenType("ROUND_LBRACE", 5);
        public static SpringTokenType ROUND_RBRACE = new SpringTokenType("ROUND_RBRACE", 6);
        public static SpringTokenType SQUARE_LBRACE = new SpringTokenType("SQUARE_LBRACE", 7);
        public static SpringTokenType SQUARE_RBRACE = new SpringTokenType("SQUARE_RBRACE", 8);
        public static SpringTokenType COMMA = new SpringTokenType("COMMA", 9);
        public static SpringTokenType ADD_OP = new SpringTokenType("ADD_OP", 10);
        public static SpringTokenType MULT_OP = new SpringTokenType("MULT_OP", 11);
        public static SpringTokenType POW_OP = new SpringTokenType("POW_OP", 12);
        public static SpringTokenType EQUIV = new SpringTokenType("EQUIV", 13);
        public static SpringTokenType IMPL = new SpringTokenType("IMPL", 14);
        public static SpringTokenType OR = new SpringTokenType("OR", 15);
        public static SpringTokenType AND = new SpringTokenType("AND", 16);
        public static SpringTokenType NEG = new SpringTokenType("NEG", 17);
        public static SpringTokenType REL_OP = new SpringTokenType("REL_OP", 18);
        public static SpringTokenType IF_KEYWORD = new SpringTokenType("IF_KEYWORD", 19);
        public static SpringTokenType THEN_KEYWORD = new SpringTokenType("THEN_KEYWORD", 20);
        public static SpringTokenType ELSE_KEYWORD = new SpringTokenType("ELSE_KEYWORD", 21);
        public static SpringTokenType COLON = new SpringTokenType("COLON", 22);
        public static SpringTokenType ASSIGN = new SpringTokenType("ASSIGN", 23);
        public static SpringTokenType GOTO_KEYWORD = new SpringTokenType("GOTO_KEYWORD", 24);
        public static SpringTokenType FOR_KEYWORD = new SpringTokenType("FOR_KEYWORD", 25);
        public static SpringTokenType WHILE_KEYWORD = new SpringTokenType("WHILE_KEYWORD", 26);
        public static SpringTokenType STEP_KEYWORD = new SpringTokenType("STEP_KEYWORD", 27);
        public static SpringTokenType UNTIL_KEYWORD = new SpringTokenType("UNTIL_KEYWORD", 28);
        public static SpringTokenType DO_KEYWORD = new SpringTokenType("DO_KEYWORD", 29);
        public static SpringTokenType BEGIN_KEYWORD = new SpringTokenType("BEGIN_KEYWORD", 30);
        public static SpringTokenType END_KEYWORD = new SpringTokenType("END_KEYWORD", 31);
        public static SpringTokenType STRING = new SpringTokenType("STRING", 32);
        public static SpringTokenType IDENTIFIER = new SpringTokenType("IDENTIFIER", 33);
        public static SpringTokenType BAD_CHARACTER = new SpringTokenType("BAD_CHARACTER", 34);
        public static SpringTokenType SEMICOLON = new SpringTokenType("SEMICOLON", 35);
        public static SpringTokenType OWN_KEYWORD = new SpringTokenType("OWN_KEYWORD", 36);
        public static SpringTokenType TYPE = new SpringTokenType("TYPE", 37);
        public static SpringTokenType ARRAY_KEYWORD = new SpringTokenType("ARRAY_KEYWORD", 38);
        public static SpringTokenType SWITCH_KEYWORD = new SpringTokenType("SWITCH_KEYWORD", 39);
        public static SpringTokenType PROCEDURE_KEYWORD = new SpringTokenType("PROCEDURE_KEYWORD", 40);
        public static SpringTokenType VALUE_KEYWORD = new SpringTokenType("VALUE_KEYWORD", 41);
        public static SpringTokenType STRING_KEYWORD = new SpringTokenType("STRING_KEYWORD", 42);
        public static SpringTokenType LABEL_KEYWORD = new SpringTokenType("LABEL_KEYWORD", 43);
        
        public SpringTokenType(string s, int index) : base(s, index)
        {
        }

        public class SpringGenericToken : LeafElementBase, ITreeNode, ITokenNode
        {
            private readonly string myText;
            private SpringTokenType myType;

            public SpringGenericToken(string text, SpringTokenType tokenType)
            {
                myText = text;
                myType = tokenType;
            }
            
            public override int GetTextLength()
            {
                return myText.Length;
            }

            public override string GetText()
            {
                return myText;
            }

            public override StringBuilder GetText(StringBuilder to)
            {
                to.Append(GetText());
                return to;
            }

            public override IBuffer GetTextAsBuffer()
            {
                return (IBuffer) new StringBuffer(GetText());
            }
            
            public override string ToString()
            {
                return base.ToString() + "(type:" + myType + ", text:" + myText + ")";
            }

            public override NodeType NodeType => (NodeType) myType;
            public override PsiLanguageType Language => SpringLanguage.Instance;
            public TokenNodeType GetTokenType()
            {
                return myType;
            }
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new SpringGenericToken(buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)), this);
        }

        public override bool IsWhitespace => WHITE_SPACE_SECTION == this;
        public override bool IsComment => COMMENT_SECTION == this;
        public override bool IsStringLiteral => STRING == this;
        public override bool IsConstantLiteral => UINT_NUMBER == this ||
                                                  NON_UINT_NUMBER == this ||
                                                  LOGICAL_CONST == this;
        public override bool IsIdentifier => IDENTIFIER == this;

        public override bool IsKeyword => IF_KEYWORD == this ||
                                          THEN_KEYWORD == this ||
                                          ELSE_KEYWORD == this ||
                                          BEGIN_KEYWORD == this ||
                                          END_KEYWORD == this ||
                                          GOTO_KEYWORD == this ||
                                          FOR_KEYWORD == this ||
                                          WHILE_KEYWORD == this ||
                                          STEP_KEYWORD == this ||
                                          UNTIL_KEYWORD == this ||
                                          DO_KEYWORD == this ||
                                          OWN_KEYWORD == this ||
                                          ARRAY_KEYWORD == this ||
                                          SWITCH_KEYWORD == this ||
                                          PROCEDURE_KEYWORD == this ||
                                          VALUE_KEYWORD == this ||
                                          STRING_KEYWORD == this ||
                                          LABEL_KEYWORD == this ||
                                          TYPE == this;

        public override string TokenRepresentation { get; }
    }
}