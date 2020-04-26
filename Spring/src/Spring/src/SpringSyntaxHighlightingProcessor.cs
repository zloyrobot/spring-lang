using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;

namespace JetBrains.ReSharper.Plugins.Spring
{
    [Language(typeof(SpringLanguage))]
    internal class SpringSyntaxHighlightingManager : RiderSyntaxHighlightingManager
    {
        public override SyntaxHighlightingProcessor CreateProcessor()
        {
            return new SpringSyntaxHighlightingProcessor();
        }
    }

    
    class SpringSyntaxHighlightingProcessor : SyntaxHighlightingProcessor
    {
        protected override bool IsBlockComment(TokenNodeType tokenType)
        {
            return base.IsBlockComment(tokenType);
        }

        protected override bool IsLineComment(TokenNodeType tokenType)
        {
            return base.IsLineComment(tokenType);
            // return SpringTokenType.NUMBER == tokenType;
        }

        protected override bool IsString(TokenNodeType tokenType)
        {
            return base.IsString(tokenType);
        }

        protected override bool IsNumber(TokenNodeType tokenType)
        {
            return base.IsNumber(tokenType);
        }

        protected override bool IsKeyword(TokenNodeType tokenType)
        {
            return base.IsKeyword(tokenType);
        }
    }
}