using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class ProjectPlannerTester
    {
        [Test]
        public void can_pick_up_gem_transform()
        {
            var mother = new DataMother("gemfile");
            mother.ToPath("gems.txt").WriteContent(@"
rake,~>10.0
fuburake,~>0.5
");


            var plan = mother.RunPlanner<ProjectPlanner>();
            plan.Steps.OfType<GemReference>().ShouldHaveTheSameElementsAs(
                new GemReference("rake", "~>10.0"),
                new GemReference("fuburake", "~>0.5")
                );

            plan.FileIsUnhandled(plan.Root.AppendPath("gems.txt")).ShouldBeFalse();
        }
        
        [Test]
        public void gitignore_directive()
        {
            var mother = new DataMother("ignoring");
            mother.ToPath("ignore.txt").WriteContent(@"pak*.zip
bin
obj
");

            var plan = mother.RunPlanner<ProjectPlanner>();
            plan.FileIsUnhandled("ignoring".AppendPath("ignore.txt"))
                .ShouldBeFalse();

            plan.Steps.OfType<GitIgnoreStep>().Single()
                .Entries.OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("bin", "obj", "pak*.zip");

        }
    }
}