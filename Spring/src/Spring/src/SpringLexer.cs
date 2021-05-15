using System;
using System.Text;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringLexer : ILexer
    {
        public void Start()
        {
            _currentPosition = -1;
            Move();
            Advance();
        }

        public void Advance()
        {
            try
            {
                EatToken();
            }
            catch (Exception)
            {
                CurToken = new SpringToken(null, "");
            }
        }

        private void EatToken()
        {
            TokenStart = _currentPosition;

            if (char.IsWhiteSpace(_curChar))
            {
                var builder = new StringBuilder();
                builder.Append(_curChar);
                Move();
                while (char.IsWhiteSpace(_curChar))
                {
                    builder.Append(_curChar);
                    Move();
                }

                CurToken = new SpringToken(SpringTokenType.Whitespace, builder.ToString());
                return;
            }

            switch (_curChar)
            {
                case '\'':
                    var str = new StringBuilder("'");
                    Move();
                    while (_curChar != '\'')
                    {
                        str.Append(_curChar);
                        Move();
                    }

                    str.Append("'");

                    CurToken = new SpringToken(SpringTokenType.String, str.ToString());
                    Move();
                    return;
                case '+':
                    CurToken = new SpringToken(SpringTokenType.Plus, _curChar.ToString());
                    Move();
                    return;
                case '-':
                    CurToken = new SpringToken(SpringTokenType.Minus, _curChar.ToString());
                    Move();
                    return;
                case '*':
                    CurToken = new SpringToken(SpringTokenType.Multiply, _curChar.ToString());
                    Move();
                    return;
                case '/':
                    Move();
                    if (_curChar == '/')
                    {
                        var comment = new StringBuilder("//");
                        Move();
                        while (_curChar != '\r' && _curChar != '\n')
                        {
                            comment.Append(_curChar);
                            Move();
                        }

                        CurToken = new SpringToken(SpringTokenType.Comment, comment.ToString());
                        return;
                    }

                    CurToken = new SpringToken(SpringTokenType.Divide, "/");

                    return;
                case '(':
                    CurToken = new SpringToken(SpringTokenType.LeftParenthesis, _curChar.ToString());
                    Move();
                    return;
                case ')':
                    CurToken = new SpringToken(SpringTokenType.RightParenthesis, _curChar.ToString());
                    Move();
                    return;
                case >= '0' and <= '9':
                {
                    var num = string.Empty;
                    while (_curChar >= '0' && _curChar <= '9')
                    {
                        num += _curChar.ToString();
                        Move();
                    }

                    if (_curChar == '.')
                    {
                        num += _curChar.ToString();
                        Move();

                        if (_curChar >= '0' && _curChar <= '9')
                        {
                            while (_curChar >= '0' && _curChar <= '9')
                            {
                                num += _curChar.ToString();
                                Move();
                            }
                        }
                        else
                        {
                            CurToken = new SpringToken(SpringTokenType.BAD_CHARACTER, _curChar.ToString());
                            return;
                        }
                    }

                    CurToken = new SpringToken(SpringTokenType.NUMBER, num);
                    return;
                }
                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                {
                    var word = string.Empty;
                    word += _curChar;
                    Move();

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
                            Move();
                        }
                    }

                    if (string.Compare(word, "BEGIN", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        CurToken = new SpringToken(SpringTokenType.Begin, word);
                    }
                    else if (string.Compare(word, "END", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        CurToken = new SpringToken(SpringTokenType.End, word);
                    }
                    else if (_curChar == '(')
                    {
                        CurToken = new SpringToken(SpringTokenType.ProcedureCall, word);
                    }
                    else
                    {
                        CurToken = new SpringToken(SpringTokenType.Variable, word);
                    }

                    return;
                }
                case ';':
                    CurToken = new SpringToken(SpringTokenType.Semi, ";");
                    Move();
                    return;
                case '.':
                    CurToken = new SpringToken(SpringTokenType.Dot, ".");
                    Move();
                    return;
                case ':':
                    Move();
                    if (_curChar == '=')
                    {
                        CurToken = new SpringToken(SpringTokenType.Assignment, ":=");
                    }
                    else
                    {
                        CurToken = new SpringToken(SpringTokenType.BAD_CHARACTER, _curChar.ToString());
                        Move();
                        throw new Exception("Unexpected token");
                    }

                    Move();
                    return;
            }

            CurToken = new SpringToken(SpringTokenType.BAD_CHARACTER, _curChar.ToString());
            Move();
            throw new Exception($"Unexpected token: {_curChar}");
        }

        private void Move()
        {
            if (isEnd) throw new Exception("Ended");
            _currentPosition += 1;

            _curChar = _currentPosition < Buffer.Length ? Buffer[_currentPosition] : char.MinValue;
        }

        private char _curChar;

        public SpringToken CurToken = new(null, "");

        public SpringLexer(IBuffer buffer)
        {
            Buffer = buffer;
        }

        public object CurrentPosition
        {
            get => _currentPosition;
            set => _currentPosition = (int) value;
        }

        private int _currentPosition;

        public TokenNodeType TokenType => CurToken.GetTokenType();
        public int TokenStart { get; private set; }

        public int TokenEnd => TokenStart + CurToken.GetTextLength();
        public IBuffer Buffer { get; }
        public bool isEnd => Buffer.Length <= _currentPosition - 1;
    }
}
