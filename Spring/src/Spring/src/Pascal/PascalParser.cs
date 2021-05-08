using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Input an expression, for example:"
                              + Environment.NewLine
                              + Environment.NewLine
                              + "    begin x := 5; y := 6.5; begin z := 7; z := (2.11*-z*(5+7.18+9.23))+---x+++(-+-+--(y+2))*(-+-3+x)*(5+y)+3*(2+1);end; t := z*2 end.");
            
            var input = "    begin x := 5; y := 6.5; begin z := 7; z := (2.11*-z*(5+7.18+9.23))+---x+++(-+-+--(y+2))*(-+-3+x)*(5+y)+3*(2+1);end; t := z*2 end.";

            {
                try
                {
                    var interpreter = new Parser(input);
                    var node = interpreter.Parse();

                    Console.WriteLine();
                    Console.WriteLine(
                        $"Tree graph:{Environment.NewLine + Environment.NewLine}{node.Accept(new GraphBuilder(), GraphBuilder.InitialData)}");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(
                        $"Value:{Environment.NewLine + Environment.NewLine}{node.Accept(new ValueBuilder(), null)}");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(
                        $"Lisp form:{Environment.NewLine + Environment.NewLine}{node.Accept(new LispFormBuilder(), null)}");
                    Console.WriteLine();
                }
                catch (InvalidSyntaxException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    internal class Parser
    {
        private Token _curToken;
        private int _curPos;
        private readonly int _charCount;
        private char _curChar;
        private string Text { get; set; }

        public Parser(string text)
        {
            Text = string.IsNullOrEmpty(text) ? string.Empty : text;
            _charCount = Text.Length;
            _curToken = Token.None();

            _curPos = -1;
            Advance();
        }

        internal Node Parse()
        {
            NextToken();
            var node = GrabCompoundStatement();
            ExpectToken(TokenType.Dot);

            return node;
        }

        private Node GrabCompoundStatement()
        {
            EatToken(TokenType.Begin);
            var statementList = GrabStatementList();
            EatToken(TokenType.End);

            return new CompoundStatement(statementList);
        }

        private List<Node> GrabStatementList()
        {
            var statementList = new List<Node>();
            var statement = GrabStatement();
            statementList.Add(statement);

            while (_curToken.Type == TokenType.Semi)
            {
                EatToken(TokenType.Semi);
                statement = GrabStatement();
                statementList.Add(statement);
            }

            return statementList;
        }

        private Node GrabStatement()
        {
            Node statement;

            if (_curToken.Type == TokenType.Begin)
            {
                statement = GrabCompoundStatement();
            }
            else if (_curToken.Type == TokenType.Variable)
            {
                statement = GrabAssignStatement();
            }
            else
            {
                statement = new NoOp();
            }

            return statement;
        }

        private Node GrabAssignStatement()
        {
            var varToken = EatToken(TokenType.Variable);
            var assignToken = EatToken(TokenType.Assignment);
            var expr = GrabExpr();

            return new AssignStatement(new Variable(varToken), assignToken, expr);
        }

        private Token ExpectToken(TokenType tokenType)
        {
            if (_curToken.Type == tokenType)
            {
                return _curToken;
            }
            else
            {
                throw new InvalidSyntaxException(
                    $"Invalid syntax at position {_curPos}. Expected {tokenType} but {_curToken.Type.ToString()} is given.");
            }
        }

        private Token EatToken(TokenType tokenType)
        {
            if (_curToken.Type == tokenType)
            {
                var token = _curToken;
                NextToken();
                return token;
            }
            else
            {
                throw new InvalidSyntaxException(
                    $"Invalid syntax at position {_curPos}. Expected {tokenType} but {_curToken.Type.ToString()} is given.");
            }
        }

        private Node GrabExpr()
        {
            var left = GrabTerm();

            while (_curToken.Type == TokenType.Plus
                || _curToken.Type == TokenType.Minus)
            {
                var op = _curToken;
                NextToken();
                var right = GrabTerm();
                left = new BinOp(op, left, right);
            }

            return left;
        }

        private Node GrabTerm()
        {
            var left = GrabFactor();

            while (_curToken.Type == TokenType.Multiply
                || _curToken.Type == TokenType.Divide)
            {
                var op = _curToken;
                NextToken();
                var right = GrabFactor();
                left = new BinOp(op, left, right);
            }

            return left;
        }

        private Node GrabFactor()
        {
            if (_curToken.Type == TokenType.Plus
                || _curToken.Type == TokenType.Minus)
            {
                var node = GrabUnaryExpr();
                return node;
            }
            else if (_curToken.Type == TokenType.LeftParenthesis)
            {
                var node = GrabBracketExpr();
                return node;
            }
            else if (_curToken.Type == TokenType.Variable)
            {
                var node = GrabVariable();
                return node;
            }
            else
            {
                var token = ExpectToken(TokenType.Number);
                NextToken();
                return new Num(token);
            }
        }

        private Node GrabVariable()
        {
            var token = ExpectToken(TokenType.Variable);
            NextToken();

            return new Variable(token);
        }

        private Node GrabUnaryExpr()
        {
            var op = ExpectToken(_curToken.Type == TokenType.Plus ? TokenType.Plus : TokenType.Minus);

            NextToken();

            if (_curToken.Type == TokenType.Plus
                || _curToken.Type == TokenType.Minus)
            {
                var expr = GrabUnaryExpr();
                return new UnaryOp(op, expr);
            }
            else
            {
                var expr = GrabFactor();
                return new UnaryOp(op, expr);
            }
        }

        private Node GrabBracketExpr()
        {
            ExpectToken(TokenType.LeftParenthesis);
            NextToken();
            var node = GrabExpr();
            ExpectToken(TokenType.RightParenthesis);
            NextToken();
            return node;
        }

        private void NextToken()
        {
            if (_curChar == char.MinValue)
            {
                _curToken = Token.None();
                return;
            }

            if (_curChar == ' ')
            {
                while (_curChar != char.MinValue && _curChar == ' ')
                {
                    Advance();
                }

                if (_curChar == char.MinValue)
                {
                    _curToken = Token.None();
                    return;
                }
            }

            if (_curChar == '+')
            {
                _curToken = new Token(TokenType.Plus, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar == '-')
            {
                _curToken = new Token(TokenType.Minus, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar == '*')
            {
                _curToken = new Token(TokenType.Multiply, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar == '/')
            {
                _curToken = new Token(TokenType.Divide, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar == '(')
            {
                _curToken = new Token(TokenType.LeftParenthesis, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar == ')')
            {
                _curToken = new Token(TokenType.RightParenthesis, _curChar.ToString());
                Advance();
                return;
            }

            if (_curChar >= '0' && _curChar <= '9')
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

                _curToken = new Token(TokenType.Number, num);
                return;
            }

            if ((_curChar >= 'a' && _curChar <= 'z')
                || _curChar >= 'A' && _curChar <= 'Z')
            {
                var word = string.Empty;
                word += _curChar;
                Advance();

                if ((_curChar >= 'a' && _curChar <= 'z')
                    || (_curChar >= 'A' && _curChar <= 'Z')
                    || _curChar == '_'
                    || (_curChar >= '0' && _curChar <= '9'))
                {
                    while ((_curChar >= 'a' && _curChar <= 'z')
                        || (_curChar >= 'A' && _curChar <= 'Z')
                        || _curChar == '_'
                        || (_curChar >= '0' && _curChar <= '9'))
                    {
                        word += _curChar.ToString();
                        Advance();
                    }
                }

                if (string.Compare(word, "BEGIN", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    _curToken = new Token(TokenType.Begin, word);
                }
                else if (string.Compare(word, "END", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    _curToken = new Token(TokenType.End, word);
                }
                else
                {
                    _curToken = new Token(TokenType.Variable, word);
                }

                return;
            }

            if (_curChar == ';')
            {
                _curToken = new Token(TokenType.Semi, ";");
                Advance();
                return;
            }

            if (_curChar == '.')
            {
                _curToken = new Token(TokenType.Dot, ".");
                Advance();
                return;
            }

            if (_curChar == ':')
            {
                if (Peek() == '=')
                {
                    Advance();
                    _curToken = new Token(TokenType.Assignment, ":=");
                    Advance();
                    return;
                }
            }

            throw new InvalidSyntaxException(
                $"Invalid syntax at position {_curPos + 1}. Unexpected symbol {_curChar}");
        }

        private void Advance()
        {
            _curPos += 1;

            _curChar = _curPos < _charCount ? Text[_curPos] : char.MinValue;
        }

        private char Peek()
        {
            return _curPos + 1 < _charCount ? Text[_curPos + 1] : char.MinValue;
        }
    }

    internal class AssignStatement : Node
    {
        private readonly Variable _variable;
        private readonly Node _expr;
        private readonly Token _assignToken;

        public AssignStatement(Variable variable, Token assignToken, Node expr)
        {
            _variable = variable;
            _expr = expr;
            _assignToken = assignToken;
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitAssignStatement(_variable, _assignToken, _expr, options);
        }
    }

    internal class NoOp : Node
    {
        public override object Accept(INodeVisitor visitor, object options)
        {
            throw new ArgumentException("Wrong ");
        }
    }

    internal class CompoundStatement : Node
    {
        private readonly List<Node> _statementList;

        public CompoundStatement(List<Node> statementList)
        {
            _statementList = new List<Node>();

            foreach (var node in statementList.Where(node => !(node is NoOp)))
            {
                _statementList.Add(node);
            }
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitCompoundStatement(_statementList, options);
        }
    }

    [Serializable]
    internal class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException()
        {
        }

        public InvalidSyntaxException(string message) : base(message)
        {
        }

        public InvalidSyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    internal class GraphBuilder : INodeVisitor
    {
        private static string ReplaceLastChar(string str, char rep = ' ')
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Substring(0, str.Length - 1) + rep;
            }
            else
            {
                return "";
            }
        }

        const char LTurn = '┌';
        const char MTurn = '├';
        const char RTurn = '└';
        const char VPipe = '│';
        const string Tab = "    ";
        const string HPipe = "──";

        internal static readonly LegacyData InitialData = new LegacyData { LegacyIndent = Tab, LegacyOrientation = BranchOrientation.Right };

        private readonly StringBuilder _sb;

        public GraphBuilder()
        {
            _sb = new StringBuilder();
            _sb.AppendLine("(Root)");
        }

        public object VisitBinOp(Token op, INode left, INode right, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                left.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent) + Tab + LTurn,
                    LegacyOrientation = BranchOrientation.Left
                });
            }
            else
            {
                left.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent, VPipe) + Tab + VPipe,
                    LegacyOrientation = BranchOrientation.Left
                });
            }

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + " (" + op + ")");
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + " (" + op + ")");
            }

            if (legacyOrientation == BranchOrientation.Right)
            {
                right.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent) + Tab + RTurn,
                    LegacyOrientation = BranchOrientation.Right
                });
            }
            else
            {
                right.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent, VPipe) + Tab + VPipe,
                    LegacyOrientation = BranchOrientation.Right
                });
            }

            return _sb.ToString();
        }

        public object VisitNum(Token num, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + "  " + num);
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + "  " + num);
            }

            return _sb.ToString();
        }

        public object VisitUnaryOp(Token op, INode node, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + " (" + op + ")");
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + " (" + op + ")");
            }

            if (legacyOrientation == BranchOrientation.Right)
            {
                node.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent) + Tab + RTurn,
                    LegacyOrientation = BranchOrientation.Right
                });
            }
            else
            {
                node.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent, VPipe) + Tab + RTurn,
                    LegacyOrientation = BranchOrientation.Right
                });
            }

            return _sb.ToString();
        }

        public object VisitAssignStatement(Variable variable, Token op, Node expr, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                variable.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent) + Tab + LTurn,
                    LegacyOrientation = BranchOrientation.Left
                });
            }
            else
            {
                variable.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent, VPipe) + Tab + VPipe,
                    LegacyOrientation = BranchOrientation.Left
                });
            }

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + " (" + op + ")");
            }
            else if (legacyOrientation == BranchOrientation.Mid)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, MTurn) + HPipe + " (" + op + ")");
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + " (" + op + ")");
            }

            if (legacyOrientation == BranchOrientation.Right)
            {
                expr.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent) + Tab + RTurn,
                    LegacyOrientation = BranchOrientation.Right
                });
            }
            else
            {
                expr.Accept(this, new LegacyData
                {
                    LegacyIndent = ReplaceLastChar(legacyIndent, VPipe) + Tab + VPipe,
                    LegacyOrientation = BranchOrientation.Right
                });
            }

            return _sb.ToString();
        }

        public object VisitCompoundStatement(List<Node> statements, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + " (Compound)");
            }
            else if (legacyOrientation == BranchOrientation.Mid)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, MTurn) + HPipe + " (Compound)");
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + " (Compound)");
            }

            var childIndent = legacyIndent;
            if (legacyOrientation == BranchOrientation.Right)
            {
                childIndent = ReplaceLastChar(childIndent) + Tab;
            }
            else
            {
                childIndent = ReplaceLastChar(childIndent, VPipe) + Tab;
            }

            for (var i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];

                if (i < statements.Count - 1)
                {
                    statement.Accept(this, new LegacyData
                    {
                        LegacyIndent = childIndent,
                        LegacyOrientation = BranchOrientation.Mid
                    });
                }
                else
                {
                    statement.Accept(this, new LegacyData
                    {
                        LegacyIndent = childIndent,
                        LegacyOrientation = BranchOrientation.Right
                    });
                }
            }

            return _sb.ToString();
        }

        public object VisitVariable(Token variable, object options)
        {
            var legacyData = (LegacyData)options;

            var legacyOrientation = legacyData.LegacyOrientation;
            var legacyIndent = legacyData.LegacyIndent;

            if (legacyOrientation == BranchOrientation.Left)
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, LTurn) + HPipe + "  " + variable);
            }
            else
            {
                _sb.AppendLine(ReplaceLastChar(legacyIndent, RTurn) + HPipe + "  " + variable);
            }

            return _sb.ToString();
        }

        internal enum BranchOrientation
        {
            Mid,
            Left,
            Right
        }

        internal class LegacyData
        {
            public BranchOrientation LegacyOrientation { get; internal set; }
            public string LegacyIndent { get; internal set; }
        }
    }

    internal class ValueBuilder : INodeVisitor
    {
        readonly Dictionary<string, decimal> _varLookup = new Dictionary<string, decimal>();

        public object VisitBinOp(Token op, INode left, INode right, object options)
        {
            switch (op.Type)
            {
                case TokenType.Plus:
                    return (decimal)left.Accept(this, options) + (decimal)right.Accept(this, options);
                case TokenType.Minus:
                    return (decimal)left.Accept(this, options) - (decimal)right.Accept(this, options);
                case TokenType.Multiply:
                    return (decimal)left.Accept(this, options) * (decimal)right.Accept(this, options);
                case TokenType.Divide:
                    return (decimal)left.Accept(this, options) / (decimal)right.Accept(this, options);
                default:
                    throw new Exception($"Token of type {op.Type.ToString()} cannot be evaluated.");
            }
        }

        public object VisitNum(Token num, object options)
        {
            return decimal.Parse(num.Value);
        }

        public object VisitAssignStatement(Variable variable, Token op, Node expr, object options)
        {
            var varName = variable.ToString();
            var value = (decimal)expr.Accept(this, options);
            _varLookup[varName] = value;

            return value;
        }

        public object VisitCompoundStatement(List<Node> statements, object options)
        {
            foreach (var statement in statements)
            {
                statement.Accept(this, options);
            }

            var sb = new StringBuilder();

            foreach (var key in _varLookup.Keys)
            {
                sb.AppendLine($"{key} = {_varLookup[key]}");
            }

            return sb.ToString(0, sb.Length - 1);
        }

        public object VisitUnaryOp(Token op, INode node, object options)
        {
            switch (op.Type)
            {
                case TokenType.Plus:
                    return (decimal)node.Accept(this, options);
                case TokenType.Minus:
                    return -(decimal)node.Accept(this, options);
                default:
                    throw new Exception($"Token of type {op.Type.ToString()} cannot be evaluated.");
            }
        }

        public object VisitVariable(Token variable, object options)
        {
            var varName = variable.Value;

            if (_varLookup.ContainsKey(varName))
            {
                return _varLookup[varName];
            }
            else
            {
                throw new Exception($"Variable {varName} is not existed.");
            }
        }
    }

    internal class LispFormBuilder : INodeVisitor
    {
        public object VisitBinOp(Token op, INode left, INode right, object options)
        {
            return "(" + op + " " + left.Accept(this, options) + " " + right.Accept(this, options) + ")";
        }

        public object VisitNum(Token num, object options)
        {
            return num.ToString();
        }

        public object VisitAssignStatement(Variable variable, Token op, Node expr, object options)
        {
            return "(" + op + " " + variable.Accept(this, options) + " " + expr.Accept(this, options) + ")";
        }

        public object VisitCompoundStatement(List<Node> statements, object options)
        {
            var sb = new StringBuilder();

            foreach (var expr in statements)
            {
                sb.AppendLine(expr.Accept(this, options).ToString());
            }

            return sb.ToString(0, sb.Length - 1);
        }

        public object VisitUnaryOp(Token op, INode node, object options)
        {
            return "(" + op + " " + node.Accept(this, options) + ")";
        }

        public object VisitVariable(Token variable, object options)
        {
            return variable.ToString();
        }
    }

    internal interface INode
    {
        object Accept(INodeVisitor visitor, object options);
    }

    internal interface INodeVisitor
    {
        object VisitNum(Token num, object options);
        object VisitUnaryOp(Token op, INode node, object options);
        object VisitBinOp(Token op, INode left, INode right, object options);
        object VisitAssignStatement(Variable variable, Token op, Node expr, object options);
        object VisitCompoundStatement(List<Node> statements, object options);
        object VisitVariable(Token variable, object options);
    }

    internal abstract class Node : INode
    {
        public abstract object Accept(INodeVisitor visitor, object options);
    }

    internal class Num : Node
    {
        private Token Token { get; set; }

        public Num(Token token)
        {
            Token = token;
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitNum(Token, options);
        }
    }

    internal class Variable : Node
    {
        private Token Token { get; set; }

        public Variable(Token token)
        {
            Token = token;
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitVariable(Token, options);
        }

        public override string ToString()
        {
            return Token.ToString();
        }
    }

    internal class UnaryOp : Node
    {
        private Token Op { get; set; }
        private Node Node { get; set; }

        public UnaryOp(Token op, Node node)
        {
            Op = op;
            Node = node;
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitUnaryOp(Op, Node, options);
        }
    }

    internal class BinOp : Node
    {
        private Token Op { get; set; }
        private Node Left { get; set; }
        private Node Right { get; set; }

        public BinOp(Token op, Node left, Node right)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override object Accept(INodeVisitor visitor, object options)
        {
            return visitor.VisitBinOp(Op, Left, Right, options);
        }
    }

    internal class Token
    {
        public TokenType Type { get; private set; }
        public string Value { get; private set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        internal static Token None()
        {
            return new Token(TokenType.None, "");
        }

        public override string ToString()
        {
            return Value;
        }
    }

    internal enum TokenType
    {
        None,
        Plus,
        Minus,
        Multiply,
        Divide,
        Number,
        LeftParenthesis,
        RightParenthesis,
        Variable,
        Assignment,
        End,
        Begin,
        Dot,
        Semi
    }
}
