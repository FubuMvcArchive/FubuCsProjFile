using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{


    [TestFixture]
    public class AssemblyReferenceTester
    {
        [Test]
        public void can_write_assembly_reference_to_a_project()
        {
            var theContext = TemplatePlan.CreateClean("assembly-ref");

            theContext.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            theContext.Add(projectPlan);

            projectPlan.Add(new SystemReference("System.Configuration"));

            theContext.Execute();


            var project = CsProjFile.LoadFrom("assembly-ref".AppendPath("src","MyProject", "MyProject.csproj"));

            project.Find<AssemblyReference>("System.Configuration")
                   .ShouldNotBeNull();
        }
    }
}