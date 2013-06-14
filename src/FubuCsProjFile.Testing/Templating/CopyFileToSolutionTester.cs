using System.IO;
using FubuCore;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CopyFileToSolutionTester
    {
        [Test]
        public void copy_a_file()
        {
            var context = TemplateContext.CreateClean("copying");

            var file = "foo.txt";
            new FileSystem().WriteStringToFile(file, "foo");

            var step = new CopyFileToSolution("foo.txt", file);
            step.Alter(context);

            File.Exists("copying".AppendPath(file)).ShouldBeTrue();
        }
    }
}