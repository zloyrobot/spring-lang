using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFile : FileElementBase
    {
        public override NodeType NodeType => SpringFileNodeType.Instance;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringBlock : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BLOCK;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringNumConst : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.NUM_CONST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringVar : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.VAR;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringInnerExpr : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.INNER_EXPR;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringUnOp : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.UN_OP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringBinOp : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BIN_OP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringCondExpr : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.COND_EXPR;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringSubscriptedVar : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SUBSCRIPTED_VAR;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringSubscriptList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SUBSCRIPT_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringFuncApp : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FUNC_APP;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringActualParamList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ACTUAL_PARAM_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    public class SpringRelation : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.RELATION;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringLabelSeq : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.LABEL_SEQ;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringLabel : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.LABEL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringAssignStat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ASSIGN_STAT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringProcedureStat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PROCEDURE_STAT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringGotoStat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.GOTO_STAT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringConditionalStat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.CONDITIONAL_STAT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringForStat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FOR_STAT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringForList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FOR_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringForListElem : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FOR_LIST_ELEM;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringBlockDeclPart : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BLOCK_DECL_PART;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringBlockStatPart : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BLOCK_STAT_PART;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    public class SpringTypeDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.TYPE_DECL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringArrayDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ARRAY_DECL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringSwitchDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SWITCH_DECL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringProcDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PROCEDURE_DECL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringIdentList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.IDENT_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringSwitchList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SWITCH_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringArraySegList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ARRAY_SEG_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringArraySeg : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.ARRAY_SEG;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringBPairList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BPAIR_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringBPair : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.BPAIR;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringFormalParamsList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FORMAL_PARAMS_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringProcValuePart : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PROCEDURE_VALUE_PART;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringProcSpecList : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PROCEDURE_SPEC_LIST;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringSpecification : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.SPECIFICATION;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringIncompleteDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.INCOMPLETE_DECLARATION;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
}