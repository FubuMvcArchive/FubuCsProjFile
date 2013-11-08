using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class ProjectDirectoryTester
    {
        private CsProjFile csProjFile;

        [SetUp]
        public void SetUp()
        {
            TemplateLibrary.FileSystem.DeleteDirectory("temp-solution");
            TemplateLibrary.FileSystem.CreateDirectory("temp-solution");

            csProjFile = CsProjFile.CreateAtSolutionDirectory("MyProj", "temp-solution");
        
        
        }

        [Test]
        public void simple_directory_does_not_already_exist()
        {
            var dir = new ProjectDirectory("content");

            dir.Alter(csProjFile, null);

            TemplateLibrary.FileSystem.DirectoryExists("temp-solution", "MyProj", "content")
                .ShouldBeTrue();
        }

        [Test]
        public void no_problem_if_the_directory_already_exists()
        {
            var dir = new ProjectDirectory("content");

            TemplateLibrary.FileSystem.CreateDirectory("temp-solution", "MyProj", "content");

            dir.Alter(csProjFile, null);

            TemplateLibrary.FileSystem.DirectoryExists("temp-solution", "MyProj", "content")
                .ShouldBeTrue();
        }

        [Test]
        public void deep_directory_does_not_already_exist()
        {
            var dir = new ProjectDirectory("content/scripts");

            dir.Alter(csProjFile, null);

            TemplateLibrary.FileSystem.DirectoryExists("temp-solution", "MyProj", "content", "scripts")
                .ShouldBeTrue();
        }

        [Test]
        public void plan_for_directory()
        {
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory( csProjFile.ProjectDirectory);
            fileSystem.CleanDirectory(csProjFile.ProjectDirectory);
            fileSystem.CreateDirectory(csProjFile.ProjectDirectory, "foo", "bar");
            fileSystem.CreateDirectory(csProjFile.ProjectDirectory, "src");
            fileSystem.CreateDirectory(csProjFile.ProjectDirectory, "packaging", "nuget");

            var directories = ProjectDirectory.PlanForDirectory(csProjFile.ProjectDirectory);
            directories.OrderBy(x => x.RelativePath).ShouldHaveTheSameElementsAs(
                new ProjectDirectory("foo"),
                new ProjectDirectory("foo/bar"),
                new ProjectDirectory("packaging"),
                new ProjectDirectory("packaging/nuget"),
                new ProjectDirectory("src")
                );

        }
    }
}