using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringGenericToken : LeafElementBase, ITokenNode
    {
        private readonly TokenNodeType myTokenType;
        private readonly string myText;

        public SpringGenericToken(TokenNodeType tokenType, string text)
        {
            myTokenType = tokenType;
            myText = text ?? string.Empty;
        }

        public override int GetTextLength()
        {
            return myText.Length;
        }

        public override StringBuilder GetText(StringBuilder to)
        {
            to.Append(GetText());
            return to;
        }

        public override IBuffer GetTextAsBuffer()
        {
            return new StringBuffer(myText);
        }

        public override string GetText()
        {
            return myText;
        }

        public override NodeType NodeType => myTokenType;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        public TokenNodeType GetTokenType()
        {
            return myTokenType;
        }
    }
}