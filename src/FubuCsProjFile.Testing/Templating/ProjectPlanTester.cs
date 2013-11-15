using System.IO;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class ProjectPlanTester
    {
        private TemplatePlan thePlan;

        [Test]
        public void project_path_is_substituted()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            thePlan.Add(new ProjectPlan("MyProject"));

            thePlan.Execute();

            thePlan.CurrentProject.ApplySubstitutions("*%PROJECT_PATH%*")
                   .ShouldEqual("*src/MyProject/MyProject.csproj*");
        }

        [Test]
        public void project_folder_is_substituted()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            thePlan.Add(new ProjectPlan("MyProject"));

            thePlan.Execute();

            thePlan.CurrentProject.ApplySubstitutions("*%PROJECT_FOLDER%*")
                   .ShouldEqual("*src/MyProject*");
        }

        [Test]
        public void short_name_is_set_automatically()
        {
            new ProjectPlan("FubuMVC.Diagnostics")
                .Substitutions.ValueFor(ProjectPlan.SHORT_NAME)
                .ShouldEqual("Diagnostics");
        }

        [Test]
        public void short_name_is_set_automatically_2()
        {
            new ProjectPlan("FubuMVC.Authentication.Twitter")
                .Substitutions.ValueFor(ProjectPlan.SHORT_NAME)
                .ShouldEqual("Twitter");
        }


        [Test]
        public void short_name_is_set_automatically_3()
        {
            new ProjectPlan("FubuMVC")
                .Substitutions.ValueFor(ProjectPlan.SHORT_NAME)
                .ShouldEqual("FubuMVC");
        }

        [Test]
        public void alter_by_creating_new_project_from_default_template()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            thePlan.Add(new ProjectPlan("MyProject"));

            thePlan.Execute();

            var file = thePlan.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.ShouldNotBeNull();  // really just a smoke test
        }

        [Test]
        public void alter_by_creating_a_new_project_with_a_project_template_file()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            thePlan.Add(new ProjectPlan("MyProject")
            {
                ProjectTemplateFile = "Project.txt"
            });

            thePlan.Execute();

            var file = thePlan.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.All<AssemblyReference>().Any(x => x.Include == "System.Data")
                .ShouldBeTrue(); // the 'special' testing template has this reference, but the default template does not

        }

        [Test]
        public void get_namespace_shallow()
        {
            ProjectPlan.GetNamespace("Foo", "Lib1").ShouldEqual("Lib1");
            ProjectPlan.GetNamespace("Foo.cs", "Lib1").ShouldEqual("Lib1");
            ProjectPlan.GetNamespace("Sub/Foo.cs", "Lib1").ShouldEqual("Lib1.Sub");
            ProjectPlan.GetNamespace("Sub/Foo", "Lib1").ShouldEqual("Lib1.Sub");
            ProjectPlan.GetNamespace("Sub/Other/Foo", "Lib1").ShouldEqual("Lib1.Sub.Other");
            ProjectPlan.GetNamespace("Sub/Other/Foo.cs", "Lib1").ShouldEqual("Lib1.Sub.Other");
        }

        [Test]
        public void default_dot_net_version_is_40()
        {
            new ProjectPlan("SomeProject")
                .DotNetVersion.ShouldEqual(DotNetVersion.V40);
        }

        [Test]
        public void project_plan_applies_the_dot_net_version()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            thePlan.Add(projectPlan);

            thePlan.Execute();

            var file = thePlan.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.DotNetVersion.ShouldEqual(DotNetVersion.V40);
        }

        [Test]
        public void project_plan_applies_the_dot_net_version_2()
        {
            thePlan = TemplatePlan.CreateClean("create-solutionProject");
            thePlan.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            projectPlan.DotNetVersion = DotNetVersion.V45;

            thePlan.Add(projectPlan);

            thePlan.Execute();

            var file = thePlan.SourceDirectory.AppendPath("MyProject", "MyProject.csproj");
            File.Exists(file).ShouldBeTrue();

            var project = CsProjFile.LoadFrom(file);
            project.DotNetVersion.ShouldEqual(DotNetVersion.V45);
        }
    }
}