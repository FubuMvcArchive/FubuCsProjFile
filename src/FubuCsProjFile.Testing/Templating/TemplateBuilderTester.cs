using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplateBuilderTester
    {
        [Test]
        public void can_pick_up_gem_transform()
        {
            var mother = new DataMother("gemfile");
            mother.ToPath("gems.txt").WriteContent(@"
rake,~>10.0
fuburake,~>0.5
");
            var plan = mother.BuildSolutionPlan();
            plan.Steps.ShouldHaveTheSameElementsAs(
                new GemReference("rake", "~>10.0"),
                new GemReference("fuburake", "~>0.5")
                );

            plan.FileIsUnhandled(plan.Root.AppendPath("gems.txt")).ShouldBeFalse();
        }

        [Test]
        public void description_txt_is_ignored()
        {
            var mother = new DataMother("empty");
            mother.ToPath("description.txt").WriteEmpty();

            var plan = mother.BuildSolutionPlan();
            plan.FileIsUnhandled("description.txt").ShouldBeFalse();

            plan.Steps.Any().ShouldBeFalse();
        }

        [Test]
        public void other_files_are_copied()
        {
            var mother = new DataMother("copied");
            mother.ToPath("foo.txt").WriteEmpty();
            mother.ToPath("bar.txt").WriteEmpty();
            mother.ToPath("deep","nested","topic.txt").WriteEmpty();

            var plan = mother.BuildSolutionPlan();
            plan.Steps.ShouldHaveTheSameElementsAs(
                new CopyFileToSolution("bar.txt", "copied".AppendPath("bar.txt")),
                new CopyFileToSolution("foo.txt", "copied".AppendPath("foo.txt")),
                new CopyFileToSolution("deep/nested/topic.txt", "copied".AppendPath("deep","nested","topic.txt"))
                
                
                
                
                );
        }

        [Test]
        public void gitignore_directive()
        {
            var mother = new DataMother("ignoring");
            mother.ToPath("ignore.txt").WriteContent(@"pak*.zip
bin
obj
");

            var plan = mother.BuildSolutionPlan();
            plan.FileIsUnhandled("ignoring".AppendPath("ignore.txt"))
                .ShouldBeFalse();

            plan.Steps.Single().ShouldBeOfType<GitIgnoreStep>()
                .Entries.OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("bin", "obj", "pak*.zip");

        }
    }
}