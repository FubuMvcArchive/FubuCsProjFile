using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class ProjectTemplateTester
    {
        [Test]
        public void sets_the_dot_net_version_if_it_exists()
        {
            var template = new ProjectTemplate
            {
                DotNetVersion = DotNetVersion.V45,
                Template = "SomeTemplate"
            };

            template.BuildProjectRequest(new TemplateChoices
            {
                ProjectName = "SomeLib",
                
            })
                .Version.ShouldEqual(DotNetVersion.V45);


            new ProjectTemplate
            {
                DotNetVersion = DotNetVersion.V40,
                Template = "SomeTemplate"
            }.BuildProjectRequest(new TemplateChoices
            {
                ProjectName = "Foo"
            })
            .Version.ShouldEqual(DotNetVersion.V40);
        }

        [Test]
        public void uses_the_default_if_dot_net_version_is_not_set()
        {
            new ProjectTemplate
            {
                DotNetVersion = null,
                Template = "Something"
            }.BuildProjectRequest(new TemplateChoices
            {
                ProjectName = "Foo"
            })
            .Version.ShouldEqual(DotNetVersion.V40);
        }
    }
}