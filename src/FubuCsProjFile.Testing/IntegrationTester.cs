using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class IntegrationTester
    {
        private TemplateLibrary library;
        private TemplatePlan thePlan;
        private TemplateRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            library =
                new TemplateLibrary(
                    ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory().AppendPath("templates"));

            theRequest = new TemplateRequest
            {
                RootDirectory = "integrated",
                SolutionName = "MySolution"
            };

            new FileSystem().DeleteDirectory("integrated");
        }

        private void executePlan(Action<TemplateRequest> configure = null)
        {
            if (configure != null)
            {
                configure(theRequest);
            }

            thePlan = new TemplatePlanBuilder(library).BuildPlan(theRequest);

            thePlan.Execute();
        }   

        public void assertDirectoryExists(params string[] parts)
        {
            Directory.Exists("integrated".AppendPath(parts)).ShouldBeTrue();
        }

        private void assertFileExists(params string[] parts)
        {
            File.Exists("integrated".AppendPath(parts)).ShouldBeTrue();
        }

        private IList<string> readFile(params string[] parts)
        {
            assertFileExists(parts);

            return new FileSystem().ReadStringFromFile("integrated".AppendPath(parts)).ReadLines().ToList();
        }

        [Test]
        public void solution_folders()
        {
            executePlan(x => x.AddTemplate("Simple"));

            thePlan.Steps.OfType<SolutionDirectory>().Any().ShouldBeTrue();

            assertDirectoryExists("src");
            assertDirectoryExists("packaging");
            assertDirectoryExists("packaging", "nuget");
        }

        [Test]
        public void solution_files()
        {
            executePlan(x => x.AddTemplate("Simple"));

            thePlan.Steps.OfType<CopyFileToSolution>().Any().ShouldBeTrue();

            assertFileExists("something.txt");
            assertFileExists("else.txt");
            assertFileExists("packaging", "nuget", "nuspec.txt");
        }

        [Test]
        public void rake_files()
        {
            executePlan(x => x.AddTemplate("Simple"));
            thePlan.Steps.OfType<RakeFileTransform>().Any().ShouldBeTrue();

            var contents = readFile("rakefile");

            contents.ShouldContain("# the solution is MySolution at src/MySolution.sln");
        }

        [Test]
        public void read_gems_from_solution()
        {
            executePlan(x => x.AddTemplate("Simple"));
            thePlan.Steps.OfType<GemReference>().Any().ShouldBeTrue();

            var contents = readFile("Gemfile");

            contents.ShouldContain("gem \"rake\", \"~>10.0\"");
            contents.ShouldContain("gem \"fuburake\", \"~>0.5\"");
        }

        [Test]
        public void gitignore_directive_in_solution()
        {
            executePlan(x => x.AddTemplate("Simple"));

            readFile(".gitignore").ShouldContain("bin");
            readFile(".gitignore").ShouldContain("obj");
        }

        [Test]
        public void create_a_default_project()
        {
            executePlan(x => {
                x.AddTemplate("Simple");
                x.AddProjectRequest(new ProjectRequest { Name = "MyProject", Template = "Simple" });
            });

            assertFileExists("src", "MyProject", "MyProject.csproj");
        }

        [Test]
        public void create_a_project_from_a_csproj_file_template()
        {
            executePlan(x =>
            {
                x.AddTemplate("Simple");
                x.AddProjectRequest(new ProjectRequest { Name = "MyProject", Template="CustomProjFile"  });
            });

            assertFileExists("src", "MyProject", "MyProject.csproj");

            var csProjFile = CsProjFile.LoadFrom("integrated".AppendPath("src", "MyProject", "MyProject.csproj"));
            csProjFile.Find<AssemblyReference>("System.Data").ShouldNotBeNull(); // this reference is NOT in the default 
        }
    }
}