using JetBrains.ReSharper.Plugins.Spring;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.SpringTests
{
    [TestFixture]
    [TestFileExtension(".Spring")]
    public class ParserTest : ParserTestBase<SpringLanguage>
    {
        protected override string RelativeTestDataPath => "parser";

        [TestCase("test01WithShell")]
        [Test]
        public void Test1(string filename)
        {
            DoOneTest(filename);
        }
        
        [TestCase("test01")]
        [Test]
        public void Test2(string filename)
        {
            var parser = new SpringParser(new SpringLexer(null));
        }
    }
}
