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
NONNEWLINE_WHITE_SPACE_CHAR=[\ \t\b\012]
WHITE_SPACE_CHAR=[{NEWLINE}\ \t\b\012]
STRING_TEXT="\""(.)*"\""
STRING_TEXT2="\""(.)*"xx"



%% 
<YYINITIAL> "=" { return currentTokenType = SpringTokenType.EQ; }
<YYINITIAL> {DIGIT}+ { return currentTokenType = SpringTokenType.NUMBER; }	
<YYINITIAL> {STRING_TEXT} { return currentTokenType = SpringTokenType.STRING; }	
<YYINITIAL> {STRING_TEXT2} { return currentTokenType = SpringTokenType.NUMBER; }	
<YYINITIAL> . { return currentTokenType = SpringTokenType.BAD_CHARACTER; }	
