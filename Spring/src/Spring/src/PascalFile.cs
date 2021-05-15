using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFile : FileElementBase
    {
        public override NodeType NodeType => SpringFileNodeType.Instance;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class AssignmentStatement : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.AssignmentStatement;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    public class CompoundStatement : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.CompoundStatement;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    public class UnaryOp : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.UnaryOp;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    public class BinOp : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BinOp;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class Expression : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.Expression;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
}
