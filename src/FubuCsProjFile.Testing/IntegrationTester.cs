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
    }
}