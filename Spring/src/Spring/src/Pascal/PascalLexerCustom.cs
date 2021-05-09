// using System;
// using System.Collections.Generic;
// using System.Linq;
// using IDE_plugin;
// using JetBrains.ReSharper.Psi.Parsing;
// using JetBrains.Text;
//
// namespace JetBrains.ReSharper.Plugins.Spring.Pascal
// {
//     public class PascalLexerCustom : ILexer
//     {
//         public void Start()
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Advance()
//         {
//             throw new NotImplementedException();
//         }
//
//         public object CurrentPosition { get; set; }
//         public TokenNodeType TokenType { get; }
//         public int TokenStart { get; }
//         public int TokenEnd { get; }
//         public IBuffer Buffer { get; }
//         public IEnumerable<Token> Tokenize(string program)
//         {
//             var tokens = new List<Token>();
//             var remainingText = program;
//             while (!string.IsNullOrWhiteSpace(remainingText))
//             { 
//                 var match = FindMatch(remainingText);
//                 if (match.Value.Length == 0 && match.TokenType == TokenType.Symbol)
//                 {
//                     throw new ArgumentException("Could not parse the argument");
//                 }
//                 tokens.Add(new Token(match.TokenType, match.Value));
//                 remainingText = match.RemainingText;
//             }
//
//             return tokens;
//         }
//
//         private static string WhiteSpaced(string val)
//         {
//             return "\\s*" + val + "\\s*";
//         }
//
//         private static readonly List<TokenDefinition> TokenDefinitions = new()
//         {
//             new TokenDefinition(TokenType.Comment, WhiteSpaced("^//(.*?)(\n|$)")),
//             new TokenDefinition(TokenType.Comment, WhiteSpaced("^\\(\\*(.*?)\\*\\)")),
//             new TokenDefinition(TokenType.Comment, WhiteSpaced("^{(.*?)}")),
//             new TokenDefinition(TokenType.CharacterString, WhiteSpaced("^'(([^']|'#\\d+')*)'|''")),
//             new TokenDefinition(TokenType.Number, WhiteSpaced("^([+-]?([0-9]*[.])?[0-9]+)")),
//             new TokenDefinition(TokenType.Identifier, WhiteSpaced("^(\\w(\\w|\\d)*)")),
//             new TokenDefinition(TokenType.Symbol, WhiteSpaced("^(\\;|'|\\+|-|\\*|\\/|=|<|>|\\[|\\]|\\.|,|\\(|\\)|:|^|@|{|}|$|#|&|%<<|>>|\\*\\*|<>|><|<=|>=|:=|\\+=|-=|\\*=|\\/=|\\(\\*|\\*\\)|\\(\\.|\\.\\)|\\)\\/\\/|\\w|[0-9])"))
//         };
//
//         private static TokenMatch FindMatch(string lqlText)
//         {
//             foreach (var match in TokenDefinitions.Select(tokenDefinition => tokenDefinition.Match(lqlText))
//                 .Where(match => match.IsMatch))
//             {
//                 return match;
//             }
//
//             return new TokenMatch {IsMatch = false};
//         }
//     }
// }
