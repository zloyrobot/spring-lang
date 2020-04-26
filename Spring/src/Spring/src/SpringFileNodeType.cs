using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class SpringFileNodeType : CompositeNodeType
    {
        public SpringFileNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringFileNodeType Instance = new SpringFileNodeType("Spring_FILE", 0);

        public override CompositeElement Create()
        {
            return new SpringFile();
        }
    }
    internal class SpringCompositeNodeType : CompositeNodeType
    {
        public SpringCompositeNodeType(string s, int index) : base(s, index)
        {
        }
        public static readonly SpringCompositeNodeType BLOCK = new SpringCompositeNodeType("BLOCK", 0);
        public static readonly SpringCompositeNodeType NUM_CONST = new SpringCompositeNodeType("NUM_CONST", 1);
        public static readonly SpringCompositeNodeType VAR = new SpringCompositeNodeType("VAR", 2);
        public static readonly SpringCompositeNodeType INNER_EXPR = new SpringCompositeNodeType("INNER_EXPR", 3);
        public static readonly SpringCompositeNodeType UN_OP = new SpringCompositeNodeType("UN_OP", 4);
        public static readonly SpringCompositeNodeType BIN_OP = new SpringCompositeNodeType("BIN_OP", 5);
        public static readonly SpringCompositeNodeType COND_EXPR = new SpringCompositeNodeType("COND_EXPR", 6);
        public static readonly SpringCompositeNodeType SUBSCRIPTED_VAR = new SpringCompositeNodeType("SUBSCRIPTED_VAR", 7);
        public static readonly SpringCompositeNodeType SUBSCRIPT_LIST = new SpringCompositeNodeType("SUBSCRIPT_LIST", 8);
        public static readonly SpringCompositeNodeType FUNC_APP = new SpringCompositeNodeType("FUNC_APP", 9);
        public static readonly SpringCompositeNodeType ACTUAL_PARAM_LIST = new SpringCompositeNodeType("ACTUAL_PARAM_LIST", 10);
        public static readonly SpringCompositeNodeType RELATION = new SpringCompositeNodeType("RELATION", 11);
        public static readonly SpringCompositeNodeType LABEL_SEQ = new SpringCompositeNodeType("LABEL_SEQ", 12);
        public static readonly SpringCompositeNodeType LABEL = new SpringCompositeNodeType("LABEL", 13);
        public static readonly SpringCompositeNodeType ASSIGN_STAT = new SpringCompositeNodeType("ASSIGN_STAT", 14);
        public static readonly SpringCompositeNodeType PROCEDURE_STAT = new SpringCompositeNodeType("PROCEDURE_STAT", 15);
        public static readonly SpringCompositeNodeType GOTO_STAT = new SpringCompositeNodeType("GOTO_STAT", 16);
        public static readonly SpringCompositeNodeType CONDITIONAL_STAT = new SpringCompositeNodeType("CONDITIONAL_STAT", 17);
        public static readonly SpringCompositeNodeType FOR_STAT = new SpringCompositeNodeType("FOR_STAT", 18);
        public static readonly SpringCompositeNodeType FOR_LIST = new SpringCompositeNodeType("FOR_LIST", 19);
        public static readonly SpringCompositeNodeType FOR_LIST_ELEM = new SpringCompositeNodeType("FOR_LIST_ELEM", 20);
        public static readonly SpringCompositeNodeType BLOCK_DECL_PART = new SpringCompositeNodeType("BLOCK_DECL_PART", 21);
        public static readonly SpringCompositeNodeType BLOCK_STAT_PART = new SpringCompositeNodeType("BLOCK_STAT_PART", 22);
        public static readonly SpringCompositeNodeType TYPE_DECL = new SpringCompositeNodeType("TYPE_DECL", 23);
        public static readonly SpringCompositeNodeType ARRAY_DECL = new SpringCompositeNodeType("ARRAY_DECL", 24);
        public static readonly SpringCompositeNodeType SWITCH_DECL = new SpringCompositeNodeType("SWITCH_DECL", 25);
        public static readonly SpringCompositeNodeType PROCEDURE_DECL = new SpringCompositeNodeType("PROCEDURE_DECL", 26);
        public static readonly SpringCompositeNodeType IDENT_LIST = new SpringCompositeNodeType("IDENT_LIST", 27);
        public static readonly SpringCompositeNodeType SWITCH_LIST = new SpringCompositeNodeType("SWITCH_LIST", 28);
        public static readonly SpringCompositeNodeType ARRAY_SEG_LIST = new SpringCompositeNodeType("ARRAY_SEG_LIST", 29);
        public static readonly SpringCompositeNodeType ARRAY_SEG = new SpringCompositeNodeType("ARRAY_SEG", 30);
        public static readonly SpringCompositeNodeType BPAIR_LIST = new SpringCompositeNodeType("BPAIR_LIST", 31);
        public static readonly SpringCompositeNodeType BPAIR = new SpringCompositeNodeType("BPAIR", 32);
        public static readonly SpringCompositeNodeType FORMAL_PARAMS_LIST = new SpringCompositeNodeType("FORMAL_PARAMS_LIST", 33);
        public static readonly SpringCompositeNodeType PROCEDURE_VALUE_PART = new SpringCompositeNodeType("PROCEDURE_VALUE_PART", 34);
        public static readonly SpringCompositeNodeType PROCEDURE_SPEC_LIST = new SpringCompositeNodeType("PROCEDURE_SPEC_LIST", 35);
        public static readonly SpringCompositeNodeType SPECIFICATION = new SpringCompositeNodeType("SPECIFICATION", 36);
        public static readonly SpringCompositeNodeType INCOMPLETE_DECLARATION = new SpringCompositeNodeType("SPECIFICATION", 37);

        
        public override CompositeElement Create()
        {
            if (BLOCK == this)
                return new SpringBlock();
            if (NUM_CONST == this)
                return new SpringNumConst();
            if (VAR == this)
                return new SpringVar();
            if (INNER_EXPR == this)
                return new SpringInnerExpr();
            if (UN_OP == this)
                return new SpringUnOp();
            if (BIN_OP == this)
                return new SpringBinOp();
            if (COND_EXPR == this)
                return new SpringCondExpr();
            if (SUBSCRIPTED_VAR == this)
                return new SpringSubscriptedVar();
            if (SUBSCRIPT_LIST == this)
                return new SpringSubscriptList();
            if (FUNC_APP == this)
                return new SpringFuncApp();
            if (ACTUAL_PARAM_LIST == this)
                return new SpringActualParamList();
            if (RELATION == this)
                return new SpringRelation();
            if (LABEL_SEQ == this)
                return new SpringLabelSeq();
            if (LABEL == this)
                return new SpringLabel();
            if (ASSIGN_STAT == this)
                return new SpringAssignStat();
            if (PROCEDURE_STAT == this)
                return new SpringProcedureStat();
            if (GOTO_STAT == this)
                return new SpringGotoStat();
            if (CONDITIONAL_STAT == this)
                return new SpringConditionalStat();
            if (FOR_STAT == this)
                return new SpringForStat();
            if (FOR_LIST == this)
                return new SpringForList();
            if (FOR_LIST_ELEM == this)
                return new SpringForListElem();
            if (BLOCK_DECL_PART == this)
                return new SpringBlockDeclPart();
            if (BLOCK_STAT_PART == this)
                return new SpringBlockStatPart();
            if (TYPE_DECL == this)
                return new SpringTypeDecl();
            if (ARRAY_DECL == this)
                return new SpringArrayDecl();
            if (SWITCH_DECL == this)
                return new SpringSwitchDecl();
            if (PROCEDURE_DECL == this)
                return new SpringProcDecl();
            if (IDENT_LIST == this)
                return new SpringIdentList();
            if (SWITCH_LIST == this)
                return new SpringSwitchList();
            if (ARRAY_SEG_LIST == this)
                return new SpringArraySegList();
            if (ARRAY_SEG == this)
                return new SpringArraySeg();
            if (BPAIR_LIST == this)
                return new SpringBPairList();
            if (BPAIR == this)
                return new SpringBPair();
            if (FORMAL_PARAMS_LIST == this)
                return new SpringFormalParamsList();
            if (PROCEDURE_VALUE_PART == this)
                return new SpringProcValuePart();
            if (PROCEDURE_SPEC_LIST == this)
                return new SpringProcSpecList();
            if (SPECIFICATION == this)
                return new SpringSpecification();
            if (INCOMPLETE_DECLARATION == this)
                return new SpringIncompleteDecl();
            throw new InvalidOperationException();
        }
    }

}