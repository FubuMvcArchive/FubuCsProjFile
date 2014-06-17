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
        
        /// <summary>
        /// 2013 introduces to new lines after the #Visual Studio 2013 line
        /// VisualStudioVersion = 12.0.21005.1
        /// MinimumVisualStudioVersion = 10.0.40219.1
        /// </summary>
        [Test]
        public void write_a_2013_solution_should_preserve_new_version_meta_data()
        {
            var solution = Solution.LoadFrom("BlankSolution.2013.sln");
            solution.Version.ShouldEqual("VS2013");

            solution.Save("BlankSolution.2013.saved.sln");

            File.ReadAllLines("BlankSolution.2013.saved.sln").ShouldEqual(File.ReadAllLines("BlankSolution.2013.sln"));
        }

        [Test]
        public void read_a_solution_with_projects()
        {
            
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            solution.Projects.Select(x => x.ProjectName)
                .ShouldHaveTheSameElementsAs("FubuMVC.SlickGrid", "FubuMVC.SlickGrid.Testing", "SlickGridHarness", "FubuMVC.SlickGrid.Serenity", "FubuMVC.SlickGrid.Docs");
        }

        [Test]
        public void read_a_solution_with_folders_correct_project_count()
        {
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            solution.Projects.Select(item => item.Project).Count().ShouldEqual(5);
        }

        [Test]
        public void read_a_solution_correctly_reports_2010_version()
        {
            var solution = Solution.LoadFrom("BlankSolution.2010.sln");
            solution.Version.ShouldEqual("VS2010");
        }

        [Test]
        public void read_a_solution_correctly_reports_2012_version()
        {
            var solution = Solution.LoadFrom("BlankSolution.2012.sln");
            solution.Version.ShouldEqual("VS2012");
        }

        [Test]
        public void read_a_solution_correctly_reports_2013_version()
        {
            var solution = Solution.LoadFrom("BlankSolution.2013.sln");
            solution.Version.ShouldEqual("VS2013");
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
        public void add_an_existing_project_to_a_new_solution_at_the_given_solution_folder()
        {
            var solution = Solution.CreateNew(@"solutions\sillydir", "newsolution");
            File.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", "FubuMVC.SlickGrid.Docs.csproj", true);

            var proj = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            solution.AddProject("Samples", CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj"));

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

        [Test]
        public void remove_an_existing_project_from_a_solution()
        {
            File.Copy("FubuMVC.SlickGrid.sln", "FubuMVC.SlickGrid.Temp.sln", true);
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.Temp.sln");
            int originalProjectcount = solution.Projects.Count();

            solution.RemoveProject(solution.Projects.First().Project);
            solution.Save();

            solution = Solution.LoadFrom("FubuMVC.SlickGrid.Temp.sln");
            solution.Projects.Count().ShouldEqual(originalProjectcount - 1);
        }

        [Test]
        public void ShouldNotIncludeFoldersAsProjects()
        {
            Solution.SolutionReader
                .IncludeAsProject(@"Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Solution Items"", ""Solution Items"", ""{24D46599-0083-4DFF-A42C-415A56BBFE2B}""")
                .ShouldBeFalse();
        }

        [Test]
        public void ShouldNotIncludeVisualStudioSetupProjects()
        {
            Solution.SolutionReader
                .IncludeAsProject(@"Project(""{54435603-DBB4-11D2-8724-00A0C9A8B90C}"") = ""FooSetup"", ""..\FooSetup.vdproj"", ""{336EAEF9-2092-4AB4-88D5-DFF339F4D670}""")
                .ShouldBeFalse();
        }

        [Test]
        public void ShouldNotIncludeWebSiteProjects()
        {
            Solution.SolutionReader
                .IncludeAsProject(@"Project(""{E24C65DC-7377-472B-9ABA-BC803B73C61A}"") = ""FooWeb"", ""..\FooWeb"", ""{236EAEF9-2092-4AB4-88D5-DFF339F4D670}""")
                .ShouldBeFalse();
        }

        [Test]
        public void ShouldIncludeLibraryProjects()
        {
            Solution.SolutionReader
                .IncludeAsProject(@"Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Foo"", ""..\Foo.csproj"", ""{326EAEF9-2092-4AB4-88D5-DFF339F4D670}""")
                .ShouldBeTrue();
        }

        [Test]
        public void to_string_should_provide_useful_information_inculdes_solutioname()
        {
            var solution = Solution.LoadFrom("BlankSolution.2013.sln");
            solution.ToString().ShouldContain(solution.Filename);
        }
    }
}