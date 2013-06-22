using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplatePlanTester
    {
        [Test]
        public void write_out_nuget_imports()
        {
            var plan = TemplatePlan.CreateClean("nuget-imports");

            plan.Add(new ProjectPlan("MyProject1"));
            plan.CurrentProject.NugetDeclarations.Add("FubuCore");
            plan.CurrentProject.NugetDeclarations.Add("FubuMVC.Core");
            plan.CurrentProject.NugetDeclarations.Add("RavenDb/2.0.0.0");

            plan.Add(new ProjectPlan("MyProject2"));
            plan.CurrentProject.NugetDeclarations.Add("RavenDb/2.0.0.0");

            plan.WriteNugetImports();

            TemplateLibrary.FileSystem.ReadStringFromFile("nuget-imports".AppendPath(TemplatePlan.RippleImportFile))
                .ReadLines().ShouldHaveTheSameElementsAs(
                "MyProject1: FubuCore, FubuMVC.Core, RavenDb/2.0.0.0",
                "MyProject2: RavenDb/2.0.0.0"
                );
        }

        [Test]
        public void substitutes_the_solution_name()
        {
            var plan = new TemplatePlan("root");
            plan.Solution = Solution.CreateNew("root", "MySolution");

            plan.ApplySubstitutions("*%SOLUTION_NAME%*")
                .ShouldEqual("*MySolution*");

        }

        [Test]
        public void substitutes_the_solution_path()
        {
            var plan = new TemplatePlan("root");
            plan.Solution = Solution.CreateNew("root".AppendPath("src"), "MySolution");

            plan.ApplySubstitutions("*%SOLUTION_PATH%*")
                .ShouldEqual("*src/MySolution.sln*");
        }
    }
}