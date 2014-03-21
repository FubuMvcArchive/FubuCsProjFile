using FubuCsProjFile.SolutionFile;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class ReadSolutionTester
    {
        [Test]
        public void reads_existing_solution_and_places_on_the_plan()
        {
            var plan = TemplatePlan.CreateClean("create-solution");
            var original = SolutionBuilder.CreateNew("create-solution", "MySln");
            original.Save();

            var step = new ReadSolution(original.Filename);

            step.Alter(plan);

            plan.Solution.ShouldNotBeNull();
            plan.Solution.Filename.ShouldEqual(original.Filename);
            plan.Solution.Name.ShouldEqual("MySln");
        }
    }
}