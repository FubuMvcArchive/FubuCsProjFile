using System.IO;
using FubuCsProjFile.SolutionFile;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CreateSolutionTester
    {
        [Test]
        public void create_a_new_solution()
        {
            var context = TemplatePlan.CreateClean("create-solution");
            var step = new CreateSolution("MySolution");

            context.Add(step);

            context.Execute();

            var file = context.Root.AppendPath("src", "MySolution.sln");
        
            File.Exists(file).ShouldBeTrue();

            var solution = SolutionReader.LoadFrom(file);
            solution.ShouldNotBeNull(); // really just a smoke test that we can parse it back out
        }

        [Test]
        public void default_is_vs2012()
        {
            new CreateSolution("foo").Version.ShouldEqual(SolutionFileVersioning.VS2012);
        }
    }
}