using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.TreeBuilder;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringGrammarParser
    {
        private PsiBuilder builder;

        public SpringGrammarParser(PsiBuilder psiBuilder)
        {
            builder = psiBuilder;
        }

        private void SkipNonsemanticalTokens()
        {
            while (builder.GetTokenType() == SpringTokenType.WHITE_SPACE_SECTION ||
                   builder.GetTokenType() == SpringTokenType.COMMENT_SECTION)
                builder.AdvanceLexer();
        }
        
        private void AdvanceLexerSkipNonsemantical()
        {
            builder.AdvanceLexer();
            SkipNonsemanticalTokens();
        }

        private bool ParseNonemptyList(Func<TokenNodeType, bool> matchDelimiter, Func<bool> ParseElement, string ErrorMsg = "An element expected")
        {
            if (!ParseElement())
                return false;

            while (matchDelimiter(builder.GetTokenType()))
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseElement())
                    builder.Error(ErrorMsg);
            }

            return true;
        }

        private bool ParseIdent()
        {
            if (builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                AdvanceLexerSkipNonsemantical();
                return true;
            }

            return false;
        }
        
        private bool ParseActualParam()
        {
            if (builder.GetTokenType() == SpringTokenType.STRING)
            {
                AdvanceLexerSkipNonsemantical();
                return true;
            }

            return ParseArithExpr();
        }
        
        private bool ParseVariable()
        {
            if (builder.GetTokenType() != SpringTokenType.IDENTIFIER)
                return false;
            
            var start = builder.Mark();
            AdvanceLexerSkipNonsemantical();

            if (builder.GetTokenType() == SpringTokenType.SQUARE_LBRACE)
            {
                var subsListMark = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseArithExpr, "An arithmetic expression expected"))
                    builder.Error("An arithmetic expression expected");
                
                if (builder.GetTokenType() != SpringTokenType.SQUARE_RBRACE)
                    builder.Error("A closing bracket expected");
                else
                    AdvanceLexerSkipNonsemantical();
            
                builder.DoneBeforeWhitespaces(subsListMark, SpringCompositeNodeType.SUBSCRIPT_LIST, null);
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SUBSCRIPTED_VAR, null);
            }
            else
            {
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.VAR, null);
            }
            
            return true;
        }

        private bool ParseArithPrimary()
        {
            if (builder.GetTokenType() == SpringTokenType.UINT_NUMBER ||
                builder.GetTokenType() == SpringTokenType.NON_UINT_NUMBER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.NUM_CONST, null);
                return true;
            }

            if (builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.SQUARE_LBRACE)
                {
                    var subsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseArithExpr, "An arithmetic expression expected"))
                        builder.Error("An arithmetic expression expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.SQUARE_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(subsListMark, SpringCompositeNodeType.SUBSCRIPT_LIST, null);
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SUBSCRIPTED_VAR, null);
                }
                else if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
                {
                    var paramsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseActualParam, "An actual parameter expected"))
                        builder.Error("An actual parameter expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(paramsListMark, SpringCompositeNodeType.ACTUAL_PARAM_LIST, null);
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.FUNC_APP, null);
                }
                else
                {
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.VAR, null);
                }
                
                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseArithExpr())
                    builder.Error("An inner arithmetic expression expected");

                if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                    builder.Error("A closing bracket expected");
                else
                    AdvanceLexerSkipNonsemantical();
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.INNER_EXPR, null);
                return true;
            }

            return false;
        }
        
        private bool ParseBooleanPrimary()
        {
            if (builder.GetTokenType() == SpringTokenType.LOGICAL_CONST)
            {
                AdvanceLexerSkipNonsemantical();
                return true;
            }

            // try to parse relation
            {
                var beforeRelationMark = builder.Mark();
                if (!ParseArithExpr())
                {
                    builder.RollbackTo(beforeRelationMark);
                }
                else if (builder.GetTokenType() != SpringTokenType.REL_OP)
                {
                    builder.RollbackTo(beforeRelationMark);
                }
                else
                {
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseArithExpr())
                    {
                        builder.Error("An arithmetic expression expected");
                    }
                    builder.DoneBeforeWhitespaces(beforeRelationMark, SpringCompositeNodeType.RELATION, null);
                    return true;
                }
            }

            if (builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
                {
                    var paramsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseActualParam, "An actual parameter expected"))
                        builder.Error("An actual parameter expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(paramsListMark, SpringCompositeNodeType.ACTUAL_PARAM_LIST, null);
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.FUNC_APP, null);
                }
                else
                {
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.VAR, null);
                }
                
                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseBooleanExpr())
                    builder.Error("An inner boolean expression expected");

                if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                    builder.Error("A closing bracket expected");
                else
                    AdvanceLexerSkipNonsemantical();
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.INNER_EXPR, null);
                return true;
            }

            return false;
        }
        
        private bool ParseDesignationalPrimary()
        {
            if (builder.GetTokenType() == SpringTokenType.UINT_NUMBER ||
                builder.GetTokenType() == SpringTokenType.NON_UINT_NUMBER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.NUM_CONST, null);
                return true;
            }

            if (builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.SQUARE_LBRACE)
                {
                    var subsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseArithExpr, "An arithmetic expression expected"))
                        builder.Error("An arithmetic expression expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.SQUARE_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(subsListMark, SpringCompositeNodeType.SUBSCRIPT_LIST, null);
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SUBSCRIPTED_VAR, null);
                }
                else
                {
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.VAR, null);
                }
                
                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseDesignationalExpr())
                    builder.Error("An inner arithmetic expression expected");

                if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                    builder.Error("A closing bracket expected");
                else
                    AdvanceLexerSkipNonsemantical();
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.INNER_EXPR, null);
                return true;
            }

            return false;
        }

        private bool ParseBooleanSecondary()
        {
            if (builder.GetTokenType() == SpringTokenType.NEG)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (!ParseBooleanPrimary())
                    builder.Error("A boolean primary expected");
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.UN_OP, null);
                return true;
            }

            return ParseBooleanPrimary();
        }

        private void ParseLeftAssocBinOpTail(int start, Func<TokenNodeType, bool> matchOperator, Func<bool> parsePart, string ErrorMsg = "An operand expected")
        {
            while (matchOperator(builder.GetTokenType()))
            {
                AdvanceLexerSkipNonsemantical();
                if (!parsePart())
                    builder.Error(ErrorMsg);
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.BIN_OP, null);
                builder.Precede(start);
            }
        }
        
        private bool ParseLeftAssocBinOpSeq(Func<TokenNodeType, bool> matchOperator, Func<bool> parsePart, string ErrorMsg = "An operand expected")
        {
            var start = builder.Mark();

            if (!parsePart())
            {
                builder.Drop(start);
                return false;
            }

            ParseLeftAssocBinOpTail(start, matchOperator, parsePart, ErrorMsg);
            
            builder.Drop(start);
            return true;
        }

        private bool ParsePowExpr()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.POW_OP,
                ParseArithPrimary,
                "A primary expression expected");
        }
        
        private bool ParseMultExpr()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.MULT_OP,
                ParsePowExpr,
                "A powering expression expected");
        }
        
        private bool ParseAddExpr()
        {
            var start = builder.Mark();
            
            if (builder.GetTokenType() == SpringTokenType.ADD_OP)
            {
                AdvanceLexerSkipNonsemantical();
                if (!ParseMultExpr())
                    builder.Error("A multiplying expression expected"); 
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.UN_OP, null);
                builder.Precede(start);
            }
            else if (!ParseMultExpr())
            {
                builder.Drop(start);
                return false;
            }

            ParseLeftAssocBinOpTail(start, tt => tt == SpringTokenType.ADD_OP, ParseMultExpr,
                "A multiplying expression expected");
            
            builder.Drop(start);
            return true;
        }
        
        private bool ParseBooleanFactor()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.AND,
                ParseBooleanSecondary,
                "A boolean secondary expected");
        }
        
        private bool ParseBooleanTerm()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.OR,
                ParseBooleanFactor,
                "A boolean factor expected");
        }
        
        private bool ParseBooleanImplExpr()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.IMPL,
                ParseBooleanTerm,
                "A boolean term expected");
        }
        
        private bool ParseBooleanEquivExpr()
        {
            return ParseLeftAssocBinOpSeq(
                tt => tt == SpringTokenType.EQUIV,
                ParseBooleanImplExpr,
                "A boolean implication expression expected");
        }

        private bool ParseSimpleArithExpr()
        {
            return ParseAddExpr();
        }
        
        private bool ParseSimpleBooleanExpr()
        {
            return ParseBooleanEquivExpr();
        }
        
        private bool ParseSimpleDesignationalExpr()
        {
            return ParseDesignationalPrimary();
        }

        private bool ParseConditionalExpr(Func<bool> ParseFirstBranch, Func<bool> ParseSecondBranch)
        {
            if (builder.GetTokenType() != SpringTokenType.IF_KEYWORD)
                return false;

            var start = builder.Mark();

            AdvanceLexerSkipNonsemantical();

            if (!ParseBooleanExpr())
                builder.Error("A boolean expression expected");

            if (builder.GetTokenType() != SpringTokenType.THEN_KEYWORD)
                builder.Error("\'then\' keyword expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            if (!ParseFirstBranch())
                builder.Error("A simple expression expected");

            if (builder.GetTokenType() != SpringTokenType.ELSE_KEYWORD) 
                builder.Error("\'else\' keyword expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            if (!ParseSecondBranch()) 
                builder.Error("An expression expected after \'else\'");
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.COND_EXPR, null);
            return true;
        }

        private bool ParseArithExpr()
        {
            if (ParseConditionalExpr(ParseSimpleArithExpr, ParseArithExpr))
                return true;
            return ParseSimpleArithExpr();
        }
        
        private bool ParseBooleanExpr()
        {
            if (ParseConditionalExpr(ParseSimpleBooleanExpr, ParseBooleanExpr))
                return true;
            return ParseSimpleBooleanExpr();
        }
        
        private bool ParseDesignationalExpr()
        {
            if (ParseConditionalExpr(ParseSimpleDesignationalExpr, ParseDesignationalExpr))
                return true;
            return ParseSimpleDesignationalExpr();
        }

        private bool ParseLabel()
        {
            if (builder.GetTokenType() == SpringTokenType.UINT_NUMBER ||
                builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                var start = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() != SpringTokenType.COLON)
                {
                    builder.RollbackTo(start);
                    return false;
                }
                
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.LABEL, null);
                AdvanceLexerSkipNonsemantical();
                return true;
            }

            return false;
        }

        private void ParseOptionalLabelSequence()
        {
            var start = builder.Mark();

            if (!ParseLabel())
            {
                builder.RollbackTo(start);
                return;
            }

            while (ParseLabel()) ;
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.LABEL_SEQ, null);
        }
        
        private bool ParseBasicStatement()
        {
            var start = builder.Mark();

            ParseOptionalLabelSequence();

            if (builder.GetTokenType() == SpringTokenType.IDENTIFIER)
            {
                var beforeIdent = builder.Mark();
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
                {
                    var paramsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseActualParam, "An actual parameter expected"))
                        builder.Error("An actual parameter expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(paramsListMark, SpringCompositeNodeType.ACTUAL_PARAM_LIST, null);
                    builder.DoneBeforeWhitespaces(beforeIdent, SpringCompositeNodeType.FUNC_APP, null);
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_STAT, null);
                    return true;
                }
                
                if (builder.GetTokenType() == SpringTokenType.SQUARE_LBRACE)
                {
                    var subsListMark = builder.Mark();
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseArithExpr, "An arithmetic expression expected"))
                        builder.Error("An arithmetic expression expected");
                    
                    if (builder.GetTokenType() != SpringTokenType.SQUARE_RBRACE)
                        builder.Error("A closing bracket expected");
                    else
                        AdvanceLexerSkipNonsemantical();
                
                    builder.DoneBeforeWhitespaces(subsListMark, SpringCompositeNodeType.SUBSCRIPT_LIST, null);
                    builder.DoneBeforeWhitespaces(beforeIdent, SpringCompositeNodeType.SUBSCRIPTED_VAR, null);
                }
                else
                {
                    builder.DoneBeforeWhitespaces(beforeIdent, SpringCompositeNodeType.VAR, null);
                }

                if (builder.GetTokenType() == SpringTokenType.ASSIGN)
                {
                    AdvanceLexerSkipNonsemantical();
                    
                    if (!ParseArithExpr())
                        builder.Error("An arithmetic expression expected");
                    
                    
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ASSIGN_STAT, null);
                }
                else
                {
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_STAT, null);
                }

                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.GOTO_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseDesignationalExpr())
                    builder.Error("A designational expression expected");

                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.GOTO_STAT, null);
                return true;
            }

            builder.RollbackTo(start);
            return false;
        }

        private bool ParseUnconditionalStatement()
        {
            if (ParseBlock())
                return true;
            return ParseBasicStatement();
        }

        private bool ParseForListElem()
        {
            var start = builder.Mark();

            if (!ParseArithExpr())
            {
                builder.RollbackTo(start);
                return false;
            }
            
            if (builder.GetTokenType() == SpringTokenType.WHILE_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                if (!ParseBooleanExpr())
                    builder.Error("A boolean expression expected");
            }
            else if (builder.GetTokenType() == SpringTokenType.STEP_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseArithExpr())
                    builder.Error("An arithmetic expression expected");
                
                if (builder.GetTokenType() != SpringTokenType.UNTIL_KEYWORD)
                    builder.Error("\'until\' expected");
                else
                    AdvanceLexerSkipNonsemantical();
                
                if (!ParseArithExpr())
                    builder.Error("An arithmetic expression expected");
            }
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.FOR_LIST_ELEM, null);
            return true;
        }
        
            
        private bool ParseForStatement()
        {
            var start = builder.Mark();

            ParseOptionalLabelSequence();
            
            if (builder.GetTokenType() != SpringTokenType.FOR_KEYWORD)
            {
                builder.RollbackTo(start);
                return false;
            }
            
            AdvanceLexerSkipNonsemantical();
            
            if (!ParseVariable())
                builder.Error("A variable expected");
            
            if (builder.GetTokenType() != SpringTokenType.ASSIGN)
                builder.Error("\':=\' expected");
            else
                AdvanceLexerSkipNonsemantical();

            var forListMarker = builder.Mark();
            
            if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA, ParseForListElem, "A for list element expected"))
                builder.Error("A for list element expected");
            
            builder.DoneBeforeWhitespaces(forListMarker, SpringCompositeNodeType.FOR_LIST, null);
            
            if (builder.GetTokenType() != SpringTokenType.DO_KEYWORD)
                builder.Error("\'do\' expected");
            else
                AdvanceLexerSkipNonsemantical();
                
            if (!ParseStatement())
                builder.Error("A statement expected");
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.FOR_STAT, null);

            return true;
        }
        
        private bool ParseConditionalStatement()
        {
            var start = builder.Mark();

            ParseOptionalLabelSequence();

            if (builder.GetTokenType() != SpringTokenType.IF_KEYWORD)
            {
                builder.RollbackTo(start);
                return false;
            }
            
            AdvanceLexerSkipNonsemantical();
            
            if (!ParseBooleanExpr())
                builder.Error("A boolean expression expected");
            
            if (builder.GetTokenType() != SpringTokenType.THEN_KEYWORD)
                builder.Error("\'then\' expected");
            else
                AdvanceLexerSkipNonsemantical();

            if (!ParseForStatement())
            {
                if (!ParseUnconditionalStatement())
                    builder.Error("An unconditional statement or a for-statement expected");

                if (builder.GetTokenType() == SpringTokenType.ELSE_KEYWORD)
                {
                    AdvanceLexerSkipNonsemantical();
                    
                    if (!ParseStatement())
                        builder.Error("A statement expected");
                }
            }
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.CONDITIONAL_STAT, null);

            return true;
        }

        private bool ParseStatement()
        {
            if (ParseConditionalStatement())
                return true;
            if (ParseForStatement())
                return true;
            return ParseUnconditionalStatement();
        }

        private bool ParseNonemptyListWrapped(NodeType wrapperNode, Func<TokenNodeType, bool> matchDelimiter, Func<bool> ParseElement, string ErrorMsg = "An element expected")
        {
            var start = builder.Mark();
            if (ParseNonemptyList(matchDelimiter, ParseElement, ErrorMsg))
            {
                builder.DoneBeforeWhitespaces(start, wrapperNode, null);
                return true;
            }
            
            builder.RollbackTo(start);
            return false;
        }

        private bool ParseIdentNonemptyList()
        {
            return ParseNonemptyListWrapped(SpringCompositeNodeType.IDENT_LIST, tt => tt == SpringTokenType.COMMA,
                ParseIdent, "An identifier expected");
        }

        private bool ParseTypeDeclTail()
        {
            return ParseIdentNonemptyList();
        }

        private bool ParseBPair()
        {
            var start = builder.Mark();
            if (!ParseArithExpr())
            {
                builder.RollbackTo(start);
                return false;
            }
            
            if (builder.GetTokenType() != SpringTokenType.COLON)
            {
                builder.RollbackTo(start);
                return false;
            }
            
            AdvanceLexerSkipNonsemantical();
            
            if (!ParseArithExpr())
                builder.Error("An arithmetic expression expected");

            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.BPAIR, null);
            return true;
        }

        private bool ParseArraySeg()
        {
            var start = builder.Mark();

            if (!ParseIdentNonemptyList())
            {
                builder.RollbackTo(start);
                return false;
            }
            
            var bPairListMark = builder.Mark();
            
            if (builder.GetTokenType() != SpringTokenType.SQUARE_LBRACE)
            {
                builder.RollbackTo(start);
                return false;
            }
            
            AdvanceLexerSkipNonsemantical();

            if (!ParseNonemptyList(tt => tt == SpringTokenType.COMMA,
                ParseBPair, "A B-pair expected"))
                builder.Error("A B-pair expected");
            
            if (builder.GetTokenType() != SpringTokenType.SQUARE_RBRACE)
                builder.Error("\']\' expected");
            else 
                AdvanceLexerSkipNonsemantical();

            builder.DoneBeforeWhitespaces(bPairListMark, SpringCompositeNodeType.BPAIR_LIST, null);
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ARRAY_SEG, null);

            return true;
        }
        private bool ParseArrayDeclTail()
        {
            return ParseNonemptyListWrapped(SpringCompositeNodeType.ARRAY_SEG_LIST, tt => tt == SpringTokenType.COMMA,
                ParseArraySeg, "An array segment expected");
        }
        
        private bool ParseSwitchDeclTail()
        {
            if (builder.GetTokenType() != SpringTokenType.IDENTIFIER)
                builder.Error("An identifier expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            if (builder.GetTokenType() != SpringTokenType.ASSIGN)
                builder.Error("\':=\' expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            if (!ParseNonemptyListWrapped(SpringCompositeNodeType.SWITCH_LIST, tt => tt == SpringTokenType.COMMA,
                ParseDesignationalExpr, "A designational expression expected"))
                builder.Error("A designational expression expected");
            
            return true;
        }

        private bool ParseProcedureValuePart()
        {
            if (builder.GetTokenType() != SpringTokenType.VALUE_KEYWORD)
                return false;
            
            var start = builder.Mark();
            
            AdvanceLexerSkipNonsemantical();
            
            if (!ParseIdentNonemptyList())
                builder.Error("An identifier expected");
            
            if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                builder.Error("\';\' expected");
            else
                AdvanceLexerSkipNonsemantical();

            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_VALUE_PART, null);
            return true;
        }

        private bool ParseSpecifier()
        {
            if (builder.GetTokenType() == SpringTokenType.TYPE)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (builder.GetTokenType() == SpringTokenType.ARRAY_KEYWORD)
                    AdvanceLexerSkipNonsemantical();
                else if (builder.GetTokenType() == SpringTokenType.PROCEDURE_KEYWORD)
                    AdvanceLexerSkipNonsemantical();
                
                return true;
            }
            if (builder.GetTokenType() == SpringTokenType.ARRAY_KEYWORD ||
                builder.GetTokenType() == SpringTokenType.PROCEDURE_KEYWORD ||
                builder.GetTokenType() == SpringTokenType.STRING_KEYWORD ||
                builder.GetTokenType() == SpringTokenType.LABEL_KEYWORD ||
                builder.GetTokenType() == SpringTokenType.SWITCH_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                return true;
            }

            return false;
        }

        private bool ParseSpecification()
        {
            var start = builder.Mark();

            if (!ParseSpecifier())
            {
                builder.RollbackTo(start);
                return false;
            }
            
            if (!ParseIdentNonemptyList())
                builder.Error("An identifier expected");
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SPECIFICATION, null);
            
            if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                builder.Error("\';\' expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            return true;
        }

        private void ParseProcedureSpecPart()
        {
            var start = builder.Mark();

            if (!ParseSpecification())
            {
                builder.RollbackTo(start);
                return;
            }

            while (ParseSpecification()) ;
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_SPEC_LIST, null);
        }
        
        private bool ParseProcedureDeclTail()
        {
            if (builder.GetTokenType() != SpringTokenType.IDENTIFIER)
                builder.Error("An identifier expected");
            else
                AdvanceLexerSkipNonsemantical();

            if (builder.GetTokenType() == SpringTokenType.ROUND_LBRACE)
            {
                var formalParamsMarker = builder.Mark();

                AdvanceLexerSkipNonsemantical();

                if (!ParseIdentNonemptyList())
                    builder.Error("An identifier expected");

                if (builder.GetTokenType() != SpringTokenType.ROUND_RBRACE)
                    builder.Error("\')\' expected");
                else
                    AdvanceLexerSkipNonsemantical();
            
                builder.DoneBeforeWhitespaces(formalParamsMarker, SpringCompositeNodeType.FORMAL_PARAMS_LIST, null);
            }
            
            if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                builder.Error("\';\' expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            ParseProcedureValuePart();

            ParseProcedureSpecPart();
            
            if (!ParseStatement())
                builder.Error("A statement expected");
            
            return true;
        }

        private bool ParseDeclaration()
        {
            var start = builder.Mark();

            if (builder.GetTokenType() == SpringTokenType.OWN_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (builder.GetTokenType() != SpringTokenType.TYPE)
                    builder.Error("A type expected");
                else
                    AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.ARRAY_KEYWORD)
                {
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseArrayDeclTail())
                        builder.Error("An array declaration expected");
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ARRAY_DECL, null);
                }
                else if (ParseTypeDeclTail())
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.TYPE_DECL, null);
                else
                {
                    builder.Error("A type/array declaration expected");
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.INCOMPLETE_DECLARATION, null);
                }
                

                if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                    builder.Error("\';\' expected");
                else
                    AdvanceLexerSkipNonsemantical();

                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.TYPE)
            {
                AdvanceLexerSkipNonsemantical();

                if (builder.GetTokenType() == SpringTokenType.ARRAY_KEYWORD)
                {
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseArrayDeclTail())
                        builder.Error("An array declaration expected");
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ARRAY_DECL, null);
                }
                else if (builder.GetTokenType() == SpringTokenType.PROCEDURE_KEYWORD)
                {
                    AdvanceLexerSkipNonsemantical();
                    if (!ParseProcedureDeclTail())
                        builder.Error("A procedure declaration expected");
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_DECL, null);
                }
                else if (ParseTypeDeclTail())
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.TYPE_DECL, null);
                else 
                {
                    builder.Error("A type/array/procedure declaration expected");
                    builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.INCOMPLETE_DECLARATION, null);
                }
                

                if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                    builder.Error("\';\' expected");
                else
                    AdvanceLexerSkipNonsemantical();

                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.ARRAY_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseArrayDeclTail()) 
                    builder.Error("An array declaration expected");
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.ARRAY_DECL, null);

                if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                    builder.Error("\';\' expected");
                else
                    AdvanceLexerSkipNonsemantical();

                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.SWITCH_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseSwitchDeclTail()) 
                    builder.Error("A switch declaration expected");
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.SWITCH_DECL, null);

                if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                    builder.Error("\';\' expected");
                else
                    AdvanceLexerSkipNonsemantical();

                return true;
            }
            
            if (builder.GetTokenType() == SpringTokenType.PROCEDURE_KEYWORD)
            {
                AdvanceLexerSkipNonsemantical();
                
                if (!ParseProcedureDeclTail()) 
                    builder.Error("A procedure declaration expected");
                builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.PROCEDURE_DECL, null);

                if (builder.GetTokenType() != SpringTokenType.SEMICOLON)
                    builder.Error("\';\' expected");
                else
                    AdvanceLexerSkipNonsemantical();

                return true;
            }
            
            builder.RollbackTo(start);
            return false;
        }
        
        private bool ParseBlock()
        {
            var start = builder.Mark();

            ParseOptionalLabelSequence();
            
            if (builder.GetTokenType() != SpringTokenType.BEGIN_KEYWORD)
            {
                builder.RollbackTo(start);
                return false;
            }
            
            AdvanceLexerSkipNonsemantical();

            var declarationPartMarker = builder.Mark();

            if (!ParseDeclaration())
                builder.RollbackTo(declarationPartMarker);
            else
            {
                while (ParseDeclaration()) ;
                builder.DoneBeforeWhitespaces(declarationPartMarker, SpringCompositeNodeType.BLOCK_DECL_PART, null);
            }
            
            var statementPartMarker = builder.Mark();
            
            if (!ParseNonemptyList(tt => tt == SpringTokenType.SEMICOLON, ParseStatement, "A statement expected"))
                builder.Error("A statement expected");
            
            builder.DoneBeforeWhitespaces(statementPartMarker, SpringCompositeNodeType.BLOCK_STAT_PART, null);

            if (builder.GetTokenType() != SpringTokenType.END_KEYWORD)
                builder.Error("\'end\' expected");
            else
                AdvanceLexerSkipNonsemantical();
            
            builder.DoneBeforeWhitespaces(start, SpringCompositeNodeType.BLOCK, null);

            return true;
        }

        public void ParseAll()
        {
            SkipNonsemanticalTokens();

            if (!ParseBlock())
            {
                builder.Error("A block expected");
                return;
            }

            if (!builder.Eof())
                builder.Error("Semantical tokens after the program end!");
        }
    }
}