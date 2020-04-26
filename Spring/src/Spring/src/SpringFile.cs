using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFile : FileElementBase
    {
        public override NodeType NodeType => SpringFileNodeType.Instance;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringSeq : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SEQ;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringAssign : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ASSIGN;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringLowBinop : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.LOW_BINOP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringMediumBinop : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.MEDIUM_BINOP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringHighBinop : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.HIGH_BINOP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringNumber : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.NUMBER;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringVariable : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.VARIABLE;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringString : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.STRING;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
}