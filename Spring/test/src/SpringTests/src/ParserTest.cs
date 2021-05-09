using JetBrains.ReSharper.Plugins.Spring.Pascal;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.Plugins.SpringTests
{
    [TestFixture]
    [TestFileExtension(".pas")]
    public class ParserTest : ParserTestBase<PascalLanguage>
    {
        protected override string RelativeTestDataPath => "parser";

        [TestCase("test01")]
        [Test]
        public void Test1(string filename)
        {
            DoOneTest(filename);
        }
    }
}
