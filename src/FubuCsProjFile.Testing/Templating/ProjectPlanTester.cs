using System.IO;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class ProjectPlanTester
    {
        private TemplatePlan thePlan;

        [SetUp]
        public void SetUp()
        {
            thePlan = TemplatePlan.CreateClean("create-project");
            thePlan.Add(new CreateSolution("MySolution"));
            thePlan.Add(new ProjectPlan("MyProject"));

            thePlan.Execute();

            var file = thePlan.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.ShouldNotBeNull();  // really just a smoke test
        }
    }
}