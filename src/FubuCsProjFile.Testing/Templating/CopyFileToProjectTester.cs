using System.IO;
using FubuCore;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CopyFileToProjectTester
    {
        private TemplatePlan thePlan;

        [Test]
        public void copy_a_file_to_the_right_spot()
        {
            thePlan = TemplatePlan.CreateClean("copy-file-to-project");

            thePlan.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            thePlan.Add(projectPlan);

            thePlan.FileSystem.WriteStringToFile("foo.txt", "some text");
            projectPlan.Add(new CopyFileToProject("foo.txt", "foo.txt"));

            thePlan.Execute();

            var file = FileSystem.Combine(thePlan.SourceDirectory, "MyProject", "foo.txt");
            File.Exists(file).ShouldBeTrue();
        }


        [Test]
        public void applies_substitutions   ()
        {
            thePlan = TemplatePlan.CreateClean("copy-file-to-project");

            thePlan.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            thePlan.Add(projectPlan);
            projectPlan.Substitutions.Set("%TEAM%", "Chiefs");

            thePlan.FileSystem.WriteStringToFile("foo.txt", "*%TEAM%*");
            projectPlan.Add(new CopyFileToProject("foo.txt", "foo.txt"));

            thePlan.Execute();

            var file = FileSystem.Combine(thePlan.SourceDirectory, "MyProject", "foo.txt");
            thePlan.FileSystem.ReadStringFromFile(file).ShouldEqual("*Chiefs*");
        }

        [Test]
        public void copy_a_deep_path_to_the_right_spot()
        {
            thePlan = TemplatePlan.CreateClean("copy-file-to-project");

            thePlan.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            thePlan.Add(projectPlan);

            thePlan.FileSystem.WriteStringToFile("foo.txt", "some text");
            projectPlan.Add(new CopyFileToProject("bar/folder/foo.txt", "foo.txt"));

            thePlan.Execute();

            var file = FileSystem.Combine(thePlan.SourceDirectory, "MyProject", "bar", "folder", "foo.txt");
            File.Exists(file).ShouldBeTrue();
        }

    }
}