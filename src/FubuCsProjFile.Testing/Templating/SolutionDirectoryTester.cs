using System.IO;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class SolutionDirectoryTester
    {
        [Test]
        public void creates_a_new_directory_if_it_does_not_already_exist()
        {
            var solutionDirectory = new SolutionDirectory("foo");

            var plan = TemplatePlan.CreateClean("solution");
            solutionDirectory.Alter(plan);

            Directory.Exists("solution".AppendPath("foo")).ShouldBeTrue();
        }

        [Test]
        public void create_a_deep_directory()
        {
            var solutionDirectory = new SolutionDirectory("foo/bar");

            var plan = TemplatePlan.CreateClean("solution");
            solutionDirectory.Alter(plan);

            Directory.Exists("solution".AppendPath("foo")).ShouldBeTrue();
            Directory.Exists("solution".AppendPath("foo", "bar")).ShouldBeTrue();
        }

        [Test]
        public void does_not_cause_any_problems_when_the_directory_already_exists()
        {
            var solutionDirectory = new SolutionDirectory("foo");

            var plan = TemplatePlan.CreateClean("solution");
            

            Directory.CreateDirectory("solution".AppendPath("foo"));

            solutionDirectory.Alter(plan);

            Directory.Exists("solution".AppendPath("foo")).ShouldBeTrue();
        }

        [Test]
        public void plan_for_directory()
        {
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory("deep-root");
            fileSystem.CleanDirectory("deep-root");
            fileSystem.CreateDirectory("deep-root", "foo", "bar");
            fileSystem.CreateDirectory("deep-root", "src");
            fileSystem.CreateDirectory("deep-root", "packaging", "nuget");

            var directories = SolutionDirectory.PlanForDirectory("deep-root");
            directories.OrderBy(x => x.RelativePath).ShouldHaveTheSameElementsAs(
                new SolutionDirectory("foo"),
                new SolutionDirectory("foo/bar"),
                new SolutionDirectory("packaging"),
                new SolutionDirectory("packaging/nuget"),
                new SolutionDirectory("src")
                );

        }
    }
}