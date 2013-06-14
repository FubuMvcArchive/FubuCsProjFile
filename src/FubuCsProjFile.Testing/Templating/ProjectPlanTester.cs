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
        private TemplateContext theContext;

        [SetUp]
        public void SetUp()
        {
            theContext = TemplateContext.CreateClean("create-project");
            theContext.Add(new CreateSolution("MySolution"));
            theContext.Add(new ProjectPlan("MyProject"));

            theContext.Execute();

            var file = theContext.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.ShouldNotBeNull();  // really just a smoke test
        }
    }
}