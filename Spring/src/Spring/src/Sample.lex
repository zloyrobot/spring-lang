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

%implements ILexer

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
NONNEWLINE_WHITE_SPACE_CHAR=[\ \t\b\012]
WHITE_SPACE_CHAR=({NEWLINE}|[\ \t\b\012])
STRING_TEXT="\""(.)*"\""



%% 
<YYINITIAL> {WHITE_SPACE_CHAR}* { return currentTokenType = SpringTokenType.WHITE_SPACE; }
<YYINITIAL> ":=" { return currentTokenType = SpringTokenType.ASSIGN; }
<YYINITIAL> ["+-"] { return currentTokenType = SpringTokenType.LOW_BINOP; }
<YYINITIAL> ["*/"] { return currentTokenType = SpringTokenType.MEDIUM_BINOP; }
<YYINITIAL> "^" { return currentTokenType = SpringTokenType.HIGH_BINOP; }
<YYINITIAL> {ALPHA}+ { return currentTokenType = SpringTokenType.IDENT; }
<YYINITIAL> "(" { return currentTokenType = SpringTokenType.LBRACKET; }
<YYINITIAL> ")" { return currentTokenType = SpringTokenType.RBRACKET; }
<YYINITIAL> ";" { return currentTokenType = SpringTokenType.SEQ; }
<YYINITIAL> {DIGIT}+ { return currentTokenType = SpringTokenType.NUMBER; }	
<YYINITIAL> {STRING_TEXT} { return currentTokenType = SpringTokenType.STRING; }	
<YYINITIAL> . { return currentTokenType = SpringTokenType.BAD_CHARACTER; }	
