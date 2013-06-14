using System.IO;
using FubuCore;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CopyFileToProjectTester
    {
        private TemplateContext theContext;

        [Test]
        public void copy_a_file_to_the_right_spot()
        {
            theContext = TemplateContext.CreateClean("copy-file-to-project");

            theContext.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            theContext.Add(projectPlan);

            theContext.FileSystem.WriteStringToFile("foo.txt", "some text");
            projectPlan.Add(new CopyFileToProject("foo.txt", "foo.txt"));

            theContext.Execute();

            var file = FileSystem.Combine(theContext.SourceDirectory, "MyProject", "foo.txt");
            File.Exists(file).ShouldBeTrue();
        }

        [Test]
        public void copy_a_deep_path_to_the_right_spot()
        {
            theContext = TemplateContext.CreateClean("copy-file-to-project");

            theContext.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            theContext.Add(projectPlan);

            theContext.FileSystem.WriteStringToFile("foo.txt", "some text");
            projectPlan.Add(new CopyFileToProject("bar/folder/foo.txt", "foo.txt"));

            theContext.Execute();

            var file = FileSystem.Combine(theContext.SourceDirectory, "MyProject", "bar", "folder", "foo.txt");
            File.Exists(file).ShouldBeTrue();
        }

    }
}