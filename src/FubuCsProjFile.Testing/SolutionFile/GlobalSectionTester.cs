using System.IO;
using System.Linq;
using FubuCsProjFile.SolutionFile;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.SolutionFile
{
    [TestFixture]
    public class GlobalSectionTester
    {
        private GlobalSection _section;

        [SetUp]
        public void SetUp()
        {
            _section = new GlobalSection("    GlobalSection(SolutionProperties) = preSolution");
        }

        [Test]
        public void the_declaration()
        {
            _section.Declaration.ShouldEqual("GlobalSection(SolutionProperties) = preSolution");
        }

        [Test]
        public void name()
        {
            _section.SectionName.ShouldEqual("SolutionProperties");
        }

        [Test]
        public void order()
        {
            _section.LoadingOrder.ShouldEqual(SolutionLoading.preSolution);
        }

        [TestCase(" something ", Result = "something")]
        [TestCase("something ", Result = "something")]
        [TestCase(" something", Result = "something")]
        [TestCase("\tsomething", Result = "something")]
        public string read_trims_text(string line)
        {
            _section.Read(line);
            return _section.Properties.Single();
        }

        [Test]
        public void write()
        {
            _section.Read("line1");
            _section.Read("line2");

            var writer = new StringWriter();
            _section.Write(writer);
            writer.ToString()
                .ShouldEqual("\tGlobalSection(SolutionProperties) = preSolution\r\n\t\tline1\r\n\t\tline2\r\n\tEndGlobalSection\r\n");
        }
    }
}