using System;
using System.Collections.Generic;
using FluentAssertions;
using JetBrains.Text;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Spring.test
{
    [TestFixture]
    public class BaseParserTest
    {
        private const string Program1 = @"
begin
  Write(55+  8.9);
  Write('Enter a number:'); // User should enter the number
end";

        [TestCase(Program1)]
        [Test]
        public void Test2(string content)
        {
            var parser = new SpringParser(new SpringLexer(new StringBuffer(content)));
            var tree = parser.ParseFile();
        }
        
        [TestCase(@"begin
end")]
        [Test]
        public void TestSimplestProgram(string file)
        {
            var parser = new SpringParser(new SpringLexer(new StringBuffer(file)));
            var tree = parser.ParseFile();
        }

        [TestCase(Program1)]
        [Test]
        public void TestLexer(string file)
        {
            var lexer = new SpringLexer(new StringBuffer(file));
            var actual = new List<SpringToken>();
            var expected = new List<SpringToken>
            {
                new(SpringTokenType.Whitespace, Environment.NewLine),
                new(SpringTokenType.Begin, "begin"),
                new(SpringTokenType.Whitespace, Environment.NewLine + "  "),
                new(SpringTokenType.ProcedureCall, "Write"),
                new(SpringTokenType.LeftParenthesis, "("),
                new(SpringTokenType.Number, "55"),
                new(SpringTokenType.Plus, "+"),
                new(SpringTokenType.Whitespace, "  "),
                new(SpringTokenType.Number, "8.9"),
                new(SpringTokenType.RightParenthesis, ")"),
                new(SpringTokenType.Semi, ";"),
                new(SpringTokenType.Whitespace, Environment.NewLine + "  "),
                new(SpringTokenType.ProcedureCall, "Write"),
                new(SpringTokenType.LeftParenthesis, "("),
                new(SpringTokenType.String, "'Enter a number:'"),
                new(SpringTokenType.RightParenthesis, ")"),
                new(SpringTokenType.Semi, ";"),
                new(SpringTokenType.Whitespace, " "),
                new(SpringTokenType.Comment, "// User should enter the number"),
                new(SpringTokenType.Whitespace, Environment.NewLine),
                new(SpringTokenType.End, "end"),
            };
            lexer.Start();
            while (lexer.TokenType != null)
            {
                actual.Add(lexer.CurToken);

                lexer.Advance();
            }

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
