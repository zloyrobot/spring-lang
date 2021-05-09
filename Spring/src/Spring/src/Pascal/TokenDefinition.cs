// using System.Text.RegularExpressions;
//
// namespace IDE_plugin
// {
//     public class TokenDefinition
//     {
//         public TokenDefinition(TokenType returnsToken, string regexPattern)
//         {
//             _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
//             _returnsToken = returnsToken;
//         }
//
//         private readonly Regex _regex;
//         private readonly TokenType _returnsToken;
//
//         public TokenMatch Match(string inputString)
//         {
//             var match = _regex.Match(inputString);
//             if (!match.Success) return new TokenMatch {IsMatch = false};
//             var remainingText = string.Empty;
//             if (match.Length != inputString.Length)
//                 remainingText = inputString.Substring(match.Length);
//
//             return new TokenMatch
//             {
//                 IsMatch = true,
//                 RemainingText = remainingText,
//                 TokenType = _returnsToken,
//                 Value = match.Groups[1].Value
//             };
//         }
//     }
//
//     public class Token
//     {
//         public TokenType Type { get; set; }
//         public string Value { get; set; }
//         public Token(TokenType type, string value)
//         {
//             Type = type;
//             Value = value;
//         }
//     }
//
//     public class TokenMatch
//     {
//         public bool IsMatch { get; set; }
//         public TokenType TokenType { get; set; }
//         public string Value { get; set; }
//         public string RemainingText { get; set; }
//     }
// }
