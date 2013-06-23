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
            var context = TemplatePlan.CreateClean("copying");

            var file = "foo.txt";
            new FileSystem().WriteStringToFile(file, "foo");

            var step = new CopyFileToSolution("foo.txt", file);
            step.Alter(context);

            File.Exists("copying".AppendPath(file)).ShouldBeTrue();
        }

        [Test]
        public void copy_a_file_applies_substitutions()
        {
            var context = TemplatePlan.CreateClean("copying");
            context.Solution = Solution.CreateNew("copying".AppendPath("src"), "FooSolution");

            var file = "foo.txt";
            new FileSystem().WriteStringToFile(file, "*%SOLUTION_NAME%*");

            var step = new CopyFileToSolution("foo.txt", file);
            step.Alter(context);

            var expectedFile = "copying".AppendPath(file);
            File.Exists(expectedFile).ShouldBeTrue();

            new FileSystem().ReadStringFromFile(expectedFile)
                            .ShouldEqual("*FooSolution*");
        }
    }
}