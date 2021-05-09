using JetBrains.ReSharper.Plugins.Pascal;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    public class PascalFile : FileElementBase
    {
        public override NodeType NodeType => PascalCompositeNodeType.Instance;

        public override PsiLanguageType Language => PascalLanguage.Instance;
    }

    public class AssignmentStatement : CompositeElement
    {
        public override NodeType NodeType => PascalCompositeNodeType.AssignmentStatement;

        public override PsiLanguageType Language => PascalLanguage.Instance;
    }
    public class CompoundStatement : CompositeElement
    {
        public override NodeType NodeType => PascalCompositeNodeType.CompoundStatement;

        public override PsiLanguageType Language => PascalLanguage.Instance;
    }
    public class UnaryOp : CompositeElement
    {
        public override NodeType NodeType => PascalCompositeNodeType.UnaryOp;

        public override PsiLanguageType Language => PascalLanguage.Instance;
    }
    public class BinOp : CompositeElement
    {
        public override NodeType NodeType => PascalCompositeNodeType.BinOp;

        public override PsiLanguageType Language => PascalLanguage.Instance;
    }
}
