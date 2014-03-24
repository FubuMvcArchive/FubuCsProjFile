using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;
using System.Linq;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class SolutionTester
    {
        [Test]
        public void default_version_is_VS2010_for_now()
        {
            Solution.CreateNew("foo", "Foo")
                .Version.ShouldEqual(Solution.VS2010);
        }

        [Test]
        public void create_new_and_read_build_configurations()
        {
            var solution = Solution.CreateNew(".", "foo");
            solution.Configurations().ShouldHaveTheSameElementsAs(
                new BuildConfiguration("Debug|Any CPU = Debug|Any CPU"),
                new BuildConfiguration("Debug|x86 = Debug|x86"),
                new BuildConfiguration("Release|Any CPU = Release|Any CPU"),
                new BuildConfiguration("Release|x86 = Release|x86")
                
                
                
                );
        }

        [Test]
        public void create_new_and_read_other_globals()
        {
            var solution = Solution.CreateNew(".", "foo");
            solution.FindSection("SolutionProperties").Properties
                .ShouldHaveTheSameElementsAs("HideSolutionNode = FALSE");
        }

        [Test]
        public void write_a_solution()
        {
            var solution = Solution.CreateNew(".".ToFullPath(), "foo");
            solution.Save();

            var original =
                new FileSystem().ReadStringFromFile(
                    ".".ToFullPath()
                       .ParentDirectory()
                       .ParentDirectory()
                       .ParentDirectory()
                       .AppendPath("FubuCsProjFile", "Solution.txt")).SplitOnNewLine();



            var newContent = new FileSystem().ReadStringFromFile("foo.sln").Trim().SplitOnNewLine();

            // skipping 2 is to ignore the version content
            newContent.Skip(2).ShouldHaveTheSameElementsAs(original);

        }

        /// <summary>
        /// Visual studio produces solution files with an empty last line.
        /// To minimize version control changes, lets mimic this behaviour.
        /// </summary>
        [Test]
        public void write_a_solution_should_end_in_blank_line()
        {
            var solution = Solution.CreateNew(".".ToFullPath(), "foo");
            solution.Save();

            new FileSystem().ReadStringFromFile("foo.sln").ShouldEndWith(Environment.NewLine);
        }

        [Test]
        public void read_a_solution_with_projects()
        {
            
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            solution.Projects.Select(x => x.ProjectName)
                .ShouldHaveTheSameElementsAs("Solution Items", "FubuMVC.SlickGrid", "FubuMVC.SlickGrid.Testing", "SlickGridHarness", "FubuMVC.SlickGrid.Serenity", "FubuMVC.SlickGrid.Docs");
        }

        [Test]
        public void read_and_write_a_solution_with_projects()
        {
            // SAMPLE: Loading-and-Saving
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            solution.Save("fake.sln");
            // ENDSAMPLE

            var original =
                new FileSystem().ReadStringFromFile("FubuMVC.SlickGrid.sln").SplitOnNewLine()
                .Select(x => x.Replace('\\', '/'));

            var newContent = new FileSystem().ReadStringFromFile("fake.sln").SplitOnNewLine().Select(x => x.Replace('\\', '/'));

            newContent.Each(x => Debug.WriteLine(x));

            newContent.ShouldHaveTheSameElementsAs(original);
        }

        [Test]
        public void adding_a_project_is_idempotent()
        {
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            var projectName = solution.Projects.Last().ProjectName;

            var initialCount = solution.Projects.Count();

            solution.AddProject(projectName);
            solution.AddProject(projectName);
            solution.AddProject(projectName);
            solution.AddProject(projectName);

            solution.Projects.Count().ShouldEqual(initialCount);

        }

        [Test]
        public void add_a_project_from_template()
        {
            // SAMPLE: create-project-by-template
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            var reference = solution.AddProjectFromTemplate("MyNewProject", Path.Combine("..", "..", "Project.txt"));
            // ENDSAMPLE

            reference.Project.Find<AssemblyReference>("System.Data")
                     .ShouldNotBeNull();

            solution.Save("foo.sln");

            // saves to the right spot
            File.Exists("MyNewProject".AppendPath("MyNewProject.csproj"))
                .ShouldBeTrue();
        }

        [Test]
        public void add_an_existing_project_to_a_new_solution()
        {
            var solution = Solution.CreateNew(@"solutions\sillydir", "newsolution");
            File.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", "FubuMVC.SlickGrid.Docs.csproj", true);

            solution.AddProject(CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj"));

            solution.FindProject("FubuMVC.SlickGrid.Docs").ShouldNotBeNull();
        }

        [Test]
        public void trying_to_add_a_project_from_template_that_already_exists_should_throw()
        {
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            var projectName = solution.Projects.First().ProjectName;

            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                solution.AddProjectFromTemplate(projectName, Path.Combine("..", "..", "Project.txt"));
            });
        }
    }
}