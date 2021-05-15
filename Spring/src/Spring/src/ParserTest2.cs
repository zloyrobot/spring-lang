using System.Collections.Generic;
using JetBrains.Text;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.Spring
{
    [TestFixture]
    public class ParserTest2
    {
        private const string program1 = @"
begin
  Write(55+  8.9);
  Write('Enter a number:'); // User should enter the number
end";
        [TestCase(program1)]
        [Test]
        public void Test2(string file)
        {
            var parser = new SpringParser(new SpringLexer(new StringBuffer(file)));
            var tree = parser.ParseFile();
        }
        
        [TestCase(program1)]
        [Test]
        public void TestLexer(string file)
        {
            var lexer = new SpringLexer(new StringBuffer(file));
            var lst = new List<SpringToken>();
            var lst2 = new List<string>();
            while (!lexer.isEnd)
            {
                lst.Add(lexer.CurToken);
                // lst2.Add(lexer.GetTokenText());
                lexer.Advance();
            }
            // var parser = new SpringParser(new SpringLexer(new StringBuffer(file)));
            // var tree = parser.ParseFile();
        }
        
    }
}
