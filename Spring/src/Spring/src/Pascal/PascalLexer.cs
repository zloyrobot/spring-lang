using System.Collections.Generic;
using System.IO;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    internal class PascalLexer : ILexer
    {
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Advance()
        {
            throw new System.NotImplementedException();
        }

        public object CurrentPosition { get; set; }
        public TokenNodeType TokenType { get; }
        public int TokenStart { get; }
        public int TokenEnd { get; }
        public IBuffer Buffer { get; }

        private IEnumerable<Lexem> ParseFile(string file)
        {
            var text = File.ReadAllText(file);
            var lexer = new PascalLexer();
            return lexer.Parse(text);
        }

        private readonly HashSet<string> _operators = new() {"begin", "end", "function"}; //and so on ....

        private IEnumerable<Lexem> Parse(string source)
        {
            foreach (var lex in RawParse(source))
            {
                switch (lex.Type)
                {
                    case LexemType.MultilineComment:
                    case LexemType.String:
                        yield return lex;
                        continue;
                    case LexemType.Identificator when lex.Content == "":
                        continue;
                }

                if (_operators.Contains(lex.Content))
                    lex.Type = LexemType.Operator; //есть в списке операторов? значит оператор
                else if (!char.IsLetterOrDigit(lex.Content[0])) lex.Type = LexemType.Separator; //сепаратор?

                //здесь можно обнаруживать другие типы лексем - числа, операторы , скобки и т.д.
                //...

                yield return lex;
            }
        }

        private IEnumerable<Lexem> RawParse(string source)
        {
            var lexem = new Lexem();

            foreach (var c in source)
                switch (lexem.Type)
                {
                    case LexemType.String:
                        if (c == '\'')
                        {
                            yield return lexem;
                            lexem = new Lexem();
                        }
                        else
                            lexem.Content += c;

                        break;

                    case LexemType.MultilineComment:
                        if (c == '}')
                        {
                            yield return lexem;
                            lexem = new Lexem();
                        }
                        else
                            lexem.Content += c;

                        break;

                    default:
                        switch (c)
                        {
                            //space
                            case ' ':
                            case '\t':
                            case '\r':
                            case '\n':
                                yield return lexem;
                                lexem = new Lexem {Type = LexemType.Identificator};
                                break;
                            //start of string
                            case '\'':
                                yield return lexem;
                                lexem = new Lexem {Type = LexemType.String};
                                break;
                            //start of comment
                            case '{':
                                yield return lexem;
                                lexem = new Lexem {Type = LexemType.MultilineComment};
                                break;
                            //identificator
                            default:
                                if (!char.IsLetterOrDigit(c))
                                {
                                    yield return lexem;
                                    yield return new Lexem {Content = c.ToString(), Type = LexemType.Identificator};
                                    lexem = new Lexem();
                                }
                                else
                                    lexem.Content += c;

                                break;
                        }

                        break;
                }

            yield return lexem;
        }
        
    }

    internal class Lexem
    {
        public LexemType Type;
        public string Content = "";

        public override string ToString()
        {
            return $"{Type}: {Content}";
        }
    }

    internal enum LexemType
    {
        Identificator = 0,
        String,
        MultilineComment,
        Operator,
        Separator
    }
}
