using System;
using System.Diagnostics;
using FubuCore;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class GitIgnoreStepTester
    {
        private readonly IFileSystem fileSystem = new FileSystem();
        private TemplatePlan thePlan;
        private Lazy<string[]> _contents;

        [SetUp]
        public void SetUp()
        {
            thePlan = TemplatePlan.CreateClean("gitignore");
        
            _contents = new Lazy<string[]>(() =>
            {
                return fileSystem.ReadStringFromFile("gitignore", ".gitignore").SplitOnNewLine().ToArray();
            });
        }

        private void ignoreFileHasOnce(string value)
        {
            try
            {
                _contents.Value.Where(x => x.Trim() == value).Count().ShouldEqual(1);
            }
            catch (Exception)
            {
                Debug.WriteLine("Actual:");
                _contents.Value.Each(x => Debug.WriteLine(x));

                throw;
                
            }
        }

        [Test]
        public void starting_from_scratch()
        {
            var step = new GitIgnoreStep("Gemfile.lock", "CommonAssemblyInfo.cs", "bin", "obj");

            step.Alter(thePlan);

            ignoreFileHasOnce("Gemfile.lock");
            ignoreFileHasOnce("CommonAssemblyInfo.cs");
            ignoreFileHasOnce("bin");
            ignoreFileHasOnce("obj");
        }

        [Test]
        public void starting_from_an_existing_file()
        {
            thePlan.AlterFile(".gitignore", list => list.Add("bin"));

            var step = new GitIgnoreStep("Gemfile.lock", "CommonAssemblyInfo.cs", "bin", "obj");

            step.Alter(thePlan);

            ignoreFileHasOnce("Gemfile.lock");
            ignoreFileHasOnce("CommonAssemblyInfo.cs");
            ignoreFileHasOnce("bin");
            ignoreFileHasOnce("obj");
        }
    }
}