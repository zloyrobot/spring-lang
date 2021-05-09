using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    internal class PascalLexerAdvanced : ILexer
    {
        public void Start()
        {
            _curPos = -1;
            Advance();
        }

        public void Advance()
        {
            _curPos += 1;

            _curChar = _curPos < Buffer.Length ? Buffer[_curPos] : char.MinValue;
            NextToken();
        }

        private void NextToken()
        {
            switch (_curChar)
            {
                case char.MinValue:
                    _curToken = new PascalToken(PascalTokenType.None, "");
                    return;
                case ' ':
                {
                    while (_curChar != char.MinValue && _curChar == ' ')
                    {
                        Advance();
                    }

                    if (_curChar == char.MinValue)
                    {
                        _curToken = new PascalToken(PascalTokenType.None, "");
                        return;
                    }

                    break;
                }
            }

            switch (_curChar)
            {
                case '+':
                    _curToken = new PascalToken(PascalTokenType.Plus, _curChar.ToString());
                    Advance();
                    return;
                case '-':
                    _curToken = new PascalToken(PascalTokenType.Minus, _curChar.ToString());
                    Advance();
                    return;
                case '*':
                    _curToken = new PascalToken(PascalTokenType.Multiply, _curChar.ToString());
                    Advance();
                    return;
                case '/':
                    _curToken = new PascalToken(PascalTokenType.Divide, _curChar.ToString());
                    Advance();
                    return;
                case '(':
                    _curToken = new PascalToken(PascalTokenType.LeftParenthesis, _curChar.ToString());
                    Advance();
                    return;
                case ')':
                    _curToken = new PascalToken(PascalTokenType.RightParenthesis, _curChar.ToString());
                    Advance();
                    return;
                case >= '0' and <= '9':
                {
                    var num = string.Empty;
                    while (_curChar >= '0' && _curChar <= '9')
                    {
                        num += _curChar.ToString();
                        Advance();
                    }

                    if (_curChar == '.')
                    {
                        num += _curChar.ToString();
                        Advance();

                        if (_curChar >= '0' && _curChar <= '9')
                        {
                            while (_curChar >= '0' && _curChar <= '9')
                            {
                                num += _curChar.ToString();
                                Advance();
                            }
                        }
                        else
                        {
                            throw new InvalidSyntaxException(
                                $"Invalid syntax at position {_curPos + 1}. Unexpected symbol {_curChar}");
                        }
                    }

                    _curToken = new PascalToken(PascalTokenType.Number, num);
                    return;
                }
                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                {
                    var word = string.Empty;
                    word += _curChar;
                    Advance();

                    if (_curChar >= 'a' && _curChar <= 'z'
                        || _curChar >= 'A' && _curChar <= 'Z'
                        || _curChar == '_'
                        || _curChar >= '0' && _curChar <= '9')
                    {
                        while (_curChar >= 'a' && _curChar <= 'z'
                               || _curChar >= 'A' && _curChar <= 'Z'
                               || _curChar == '_'
                               || _curChar >= '0' && _curChar <= '9')
                        {
                            word += _curChar.ToString();
                            Advance();
                        }
                    }

                    if (string.Compare(word, "BEGIN", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _curToken = new PascalToken(PascalTokenType.Begin, word);
                    }
                    else if (string.Compare(word, "END", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _curToken = new PascalToken(PascalTokenType.End, word);
                    }
                    else
                    {
                        _curToken = new PascalToken(PascalTokenType.Variable, word);
                    }

                    return;
                }
                case ';':
                    _curToken = new PascalToken(PascalTokenType.Semi, ";");
                    Advance();
                    return;
                case '.':
                    _curToken = new PascalToken(PascalTokenType.Dot, ".");
                    Advance();
                    return;
            }

            Advance();
            _curToken = new PascalToken(PascalTokenType.Assignment, ":=");
            Advance();
        }

        private int _curPos;
        private char _curChar;
        private PascalToken _curToken;

        public object CurrentPosition
        {
            get => _curPos;
            set => _curPos = (int) value;
        }

        public TokenNodeType TokenType { get; }
        public int TokenStart { get; }
        public int TokenEnd { get; }
        public IBuffer Buffer { get; }
    }
}
