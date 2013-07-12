using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuCsProjFile.Templating;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuCsProjFile.Testing
{

    [TestFixture]
    public class creating_a_new_solution_with_projects
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            var fileSystem = new FileSystem();
            fileSystem.DeleteDirectory("TestSolution");
            fileSystem.CreateDirectory("TestSolution");
        }

        [Test]
        public void create_solution_add_project_save_and_reload()
        {
            // Yeah, this is a big bang test.  Just go with it.

            var solution = Solution.CreateNew("TestSolution", "TestSolution");

            var reference = solution.AddProject("TestProject");
            reference.ProjectGuid.ShouldNotEqual(Guid.Empty);
            reference.ProjectName.ShouldEqual("TestProject");
            reference.RelativePath.ShouldEqual("TestProject".AppendPath("TestProject.csproj"));

            var plan = new ProjectPlan(reference.ProjectName);

            CodeFileTemplate.Class("Foo").Alter(reference.Project, plan);
            CodeFileTemplate.Class("Bar").Alter(reference.Project, plan);

            solution.Save();

            File.Exists("TestSolution".AppendPath("TestSolution.sln")).ShouldBeTrue();
            File.Exists("TestSolution".AppendPath("TestProject", "TestProject.csproj")).ShouldBeTrue();

            var solution2 = Solution.LoadFrom("TestSolution".AppendPath("TestSolution.sln"));
            var reference2 = solution2.FindProject("TestProject");

            reference2.ShouldNotBeNull();

            var project2 = reference2.Project;
            project2.ShouldNotBeNull();

            project2.All<CodeFile>().OrderBy(x => x.Include)
                .Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("Bar.cs", "Foo.cs");
        }

        [Test]
        public void create_solution_add_project_save_and_reload_2()
        {
            // Yeah, this is a big bang test.  Just go with it.

            var solution = Solution.CreateNew("TestSolution", "Lib1.TestSolution");

            var reference = solution.AddProject("TestProject");
            reference.ProjectGuid.ShouldNotEqual(Guid.Empty);
            reference.ProjectName.ShouldEqual("TestProject");
            reference.RelativePath.ShouldEqual("TestProject".AppendPath("TestProject.csproj"));

            var plan = new ProjectPlan(reference.ProjectName);

            CodeFileTemplate.Class("Foo").Alter(reference.Project, plan);
            CodeFileTemplate.Class("Bar").Alter(reference.Project, plan);

            solution.Save();

            File.Exists("TestSolution".AppendPath("Lib1.TestSolution.sln")).ShouldBeTrue();
            File.Exists("TestSolution".AppendPath("TestProject", "TestProject.csproj")).ShouldBeTrue();

            var solution2 = Solution.LoadFrom("TestSolution".AppendPath("Lib1.TestSolution.sln"));
            var reference2 = solution2.FindProject("TestProject");

            reference2.ShouldNotBeNull();

            var project2 = reference2.Project;
            project2.ShouldNotBeNull();

            project2.All<CodeFile>().OrderBy(x => x.Include)
                .Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("Bar.cs", "Foo.cs");
        }
    }
}