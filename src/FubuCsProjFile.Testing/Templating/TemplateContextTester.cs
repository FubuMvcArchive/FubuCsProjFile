using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplateContextTester
    {
        [Test]
        public void default_source_directory_is_src()
        {
            new TemplateContext("something").SourceName
                                            .ShouldEqual("src");
        }
    }
}