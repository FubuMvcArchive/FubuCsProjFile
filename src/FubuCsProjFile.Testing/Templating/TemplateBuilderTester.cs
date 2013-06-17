using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

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
        }

        [Test]
        public void description_txt_is_ignored()
        {
            var mother = new DataMother("empty");
            mother.ToPath("description.txt").WriteEmpty();

            mother.BuildSolutionPlan().Steps.Any().ShouldBeFalse();
        }
    }
}