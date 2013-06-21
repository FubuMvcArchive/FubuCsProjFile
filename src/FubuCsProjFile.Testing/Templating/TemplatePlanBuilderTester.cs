using FubuCsProjFile.Templating;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using FubuCore;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplatePlanBuilderTester
    {
        [SetUp]
        public void SetUp()
        {
            TemplateLibrary.FileSystem.DeleteDirectory("root");
            TemplateLibrary.FileSystem.CreateDirectory("root");
        }

        [Test]
        public void build_with_missing_solution()
        {
            var request = new TemplateRequest
            {
                SolutionName = "MySolution",
                RootDirectory = "root"
            };

            var plan = new TemplatePlanBuilder(new TemplateLibrary("."))
                .BuildPlan(request);

            var create =plan.Steps.First().ShouldBeOfType<CreateSolution>();
            create.SolutionName.ShouldEqual("MySolution");
        }

        [Test]
        public void build_with_existing_solution_should_just_read_the_current_one()
        {
            var original = Solution.CreateNew("root".AppendPath("src"), "MySolution");
            original.Save();

            var request = new TemplateRequest
            {
                SolutionName = "MySolution",
                RootDirectory = "root"
            };

            var plan = new TemplatePlanBuilder(new TemplateLibrary("."))
                .BuildPlan(request);

            var readSolution = plan.Steps.First().ShouldBeOfType<ReadSolution>();
            readSolution.SolutionFile.ToFullPath()
                        .ShouldEqual(original.Filename.ToFullPath());
        }
    }
}