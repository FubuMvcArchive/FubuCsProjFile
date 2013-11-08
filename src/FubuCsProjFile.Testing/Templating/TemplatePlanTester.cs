using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplatePlanTester
    {
        [Test]
        public void execute_blows_up_if_there_are_any_missing_inputs()
        {
            var plan = TemplatePlan.CreateClean("missing-inputs");
            plan.MissingInputs.Add("Foo");
            plan.MissingInputs.Add("Bar");

            Exception<MissingInputException>.ShouldBeThrownBy(() => {
                plan.Execute();
            }).InputNames.ShouldHaveTheSameElementsAs("Foo", "Bar");
        }

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

        [Test]
        public void add_instructions_simple()
        {
            var plan = new TemplatePlan("root");
            plan.AddInstructions("some foo");

            plan.WriteInstructions();

            new FileSystem().ReadStringFromFile("root".AppendPath(TemplatePlan.InstructionsFile))
                .ShouldContain("some foo");
        }



        [Test]
        public void add_instructions_with_substitution()
        {
            var plan = new TemplatePlan("root");
            plan.Substitutions.Set("%SOLUTION_NAME%", "Foo");
            plan.AddInstructions("the solution is '%SOLUTION_NAME%'");

            plan.WriteInstructions();

            new FileSystem().ReadStringFromFile("root".AppendPath(TemplatePlan.InstructionsFile))
                .ShouldContain("the solution is 'Foo'");
        }
    }
}