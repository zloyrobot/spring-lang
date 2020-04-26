using System;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;
using JetBrains.ReSharper.Plugins.Spring;

%%
%{
TokenNodeType currentTokenType;
%}

%unicode

%init{
  currentTokenType = null;
%init}

%namespace Sample
%class SampleLexerGenerated

%function _locateToken
%public
%type TokenNodeType
%ignorecase

%eofval{
  currentTokenType = null; return currentTokenType;
%eofval}


ALPHA=[A-Za-z]
DIGIT=[0-9]
NEWLINE=((\r\n)|\n)
WHITE_SPACE_CHAR=({NEWLINE}|[\ \t\b\012])
STRING_TEXT="\""(.)*"\""


UNSIGNED_INT={DIGIT}+
DECIMAL_NUMBER=({UNSIGNED_INT}|"."{UNSIGNED_INT}|{UNSIGNED_INT}"."{UNSIGNED_INT})
INT=({UNSIGNED_INT}|"+"{UNSIGNED_INT}|"-"{UNSIGNED_INT})
NUMBER=({DECIMAL_NUMBER}|₁₀{INT}|{DECIMAL_NUMBER}₁₀{INT})

COMMENT_SECTION=comment[^";"]*";"

%%

<YYINITIAL> {UNSIGNED_INT} { return currentTokenType = SpringTokenType.UINT_NUMBER; }
<YYINITIAL> {NUMBER} { return currentTokenType = SpringTokenType.NON_UINT_NUMBER; }
<YYINITIAL> (true|false) { return currentTokenType = SpringTokenType.LOGICAL_CONST; }
<YYINITIAL> "(" { return currentTokenType = SpringTokenType.ROUND_LBRACE; }
<YYINITIAL> ")" { return currentTokenType = SpringTokenType.ROUND_RBRACE; }
<YYINITIAL> "[" { return currentTokenType = SpringTokenType.SQUARE_LBRACE; }
<YYINITIAL> "]" { return currentTokenType = SpringTokenType.SQUARE_RBRACE; }
<YYINITIAL> "," { return currentTokenType = SpringTokenType.COMMA; }
<YYINITIAL> ["+-"] { return currentTokenType = SpringTokenType.ADD_OP; }
<YYINITIAL> ["×/÷"] { return currentTokenType = SpringTokenType.MULT_OP; }
<YYINITIAL> ↑ { return currentTokenType = SpringTokenType.POW_OP; }
<YYINITIAL> ≣ { return currentTokenType = SpringTokenType.EQUIV; }
<YYINITIAL> ⊃ { return currentTokenType = SpringTokenType.IMPL; }
<YYINITIAL> ⋁ { return currentTokenType = SpringTokenType.OR; }
<YYINITIAL> ⋀ { return currentTokenType = SpringTokenType.AND; }
<YYINITIAL> ¬ { return currentTokenType = SpringTokenType.NEG; }
<YYINITIAL> ["<≤=≠>≥"] { return currentTokenType = SpringTokenType.REL_OP; }
<YYINITIAL> if { return currentTokenType = SpringTokenType.IF_KEYWORD; }
<YYINITIAL> then { return currentTokenType = SpringTokenType.THEN_KEYWORD; }
<YYINITIAL> else { return currentTokenType = SpringTokenType.ELSE_KEYWORD; }
<YYINITIAL> ":" { return currentTokenType = SpringTokenType.COLON; }
<YYINITIAL> ";" { return currentTokenType = SpringTokenType.SEMICOLON; }
<YYINITIAL> ":=" { return currentTokenType = SpringTokenType.ASSIGN; }
<YYINITIAL> goto { return currentTokenType = SpringTokenType.GOTO_KEYWORD; }
<YYINITIAL> for { return currentTokenType = SpringTokenType.FOR_KEYWORD; }
<YYINITIAL> while { return currentTokenType = SpringTokenType.WHILE_KEYWORD; }
<YYINITIAL> step { return currentTokenType = SpringTokenType.STEP_KEYWORD; }
<YYINITIAL> until { return currentTokenType = SpringTokenType.UNTIL_KEYWORD; }
<YYINITIAL> do { return currentTokenType = SpringTokenType.DO_KEYWORD; }
<YYINITIAL> begin { return currentTokenType = SpringTokenType.BEGIN_KEYWORD; }
<YYINITIAL> end { return currentTokenType = SpringTokenType.END_KEYWORD; }
<YYINITIAL> own { return currentTokenType = SpringTokenType.OWN_KEYWORD; }
<YYINITIAL> (real|integer|boolean) { return currentTokenType = SpringTokenType.TYPE; }
<YYINITIAL> array { return currentTokenType = SpringTokenType.ARRAY_KEYWORD; }
<YYINITIAL> switch { return currentTokenType = SpringTokenType.SWITCH_KEYWORD; }
<YYINITIAL> procedure { return currentTokenType = SpringTokenType.PROCEDURE_KEYWORD; }
<YYINITIAL> value { return currentTokenType = SpringTokenType.VALUE_KEYWORD; }
<YYINITIAL> string { return currentTokenType = SpringTokenType.STRING_KEYWORD; }
<YYINITIAL> label { return currentTokenType = SpringTokenType.LABEL_KEYWORD; }

<YYINITIAL> {WHITE_SPACE_CHAR}* { return currentTokenType = SpringTokenType.WHITE_SPACE_SECTION; }
<YYINITIAL> {COMMENT_SECTION} { return currentTokenType = SpringTokenType.COMMENT_SECTION; }

<YYINITIAL> {STRING_TEXT} { return currentTokenType = SpringTokenType.STRING; }
<YYINITIAL> {ALPHA}({ALPHA}|{DIGIT})* { return currentTokenType = SpringTokenType.IDENTIFIER; }
<YYINITIAL> . { return currentTokenType = SpringTokenType.BAD_CHARACTER; }	
