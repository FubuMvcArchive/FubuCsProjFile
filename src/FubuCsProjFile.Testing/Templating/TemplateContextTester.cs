using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplateContextTester
    {
        [Test]
        public void default_source_directory_is_src()
        {
            new TemplatePlan("something").SourceName
                                            .ShouldEqual("src");
        }

        [Test]
        public void remembers_the_last_project_plan()
        {
            var context = new TemplatePlan("something");
            var plan1 = new ProjectPlan("NewProj1");
            var plan2 = new ProjectPlan("NewProj2");
        
            context.Add(new GitIgnoreStep("foo"));
            context.CurrentProject.ShouldBeNull();

            context.Add(plan1);
            context.CurrentProject.ShouldBeTheSameAs(plan1);

            context.Add(new CopyFileToSolution("foo", "foo.txt"));
            context.CurrentProject.ShouldBeTheSameAs(plan1);

            context.Add(plan2);

            context.CurrentProject.ShouldBeTheSameAs(plan2);
        }
    }
}