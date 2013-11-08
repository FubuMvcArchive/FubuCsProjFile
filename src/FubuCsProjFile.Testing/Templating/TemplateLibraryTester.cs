using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplateLibraryTester
    {
        [Test]
        public void can_read_solution_templates()
        {
            var library = TemplateLibrary.BuildClean("solutions");
            library.StartTemplate(TemplateType.Solution, "sln1");
            library.StartTemplate(TemplateType.Solution, "sln2");
            library.StartTemplate(TemplateType.Solution, "sln3");

            library.All().Where(x => x.Type == TemplateType.Solution)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("sln1", "sln2", "sln3");


        }

        [Test]
        public void can_read_project_templates()
        {
            var library = TemplateLibrary.BuildClean("projects");
            library.StartTemplate(TemplateType.Project, "proj1");
            library.StartTemplate(TemplateType.Project, "proj2");
            library.StartTemplate(TemplateType.Project, "proj3");

            library.All().Where(x => x.Type == TemplateType.Project)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("proj1", "proj2", "proj3");
        }



        [Test]
        public void can_read_alteration_templates()
        {
            var library = TemplateLibrary.BuildClean("alterations");
            library.StartTemplate(TemplateType.Alteration, "alter1");
            library.StartTemplate(TemplateType.Alteration, "alter2");
            library.StartTemplate(TemplateType.Alteration, "alter3");

            library.All().Where(x => x.Type == TemplateType.Alteration)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("alter1", "alter2", "alter3");
        }

        [Test]
        public void find_with_mixed()
        {
            var library = TemplateLibrary.BuildClean("mixed");
            library.StartTemplate(TemplateType.Alteration, "first").WriteDescription("the alteration");
            library.StartTemplate(TemplateType.Project, "first").WriteDescription("the solutionProject");
            library.StartTemplate(TemplateType.Solution, "first").WriteDescription("the solution");
            library.StartTemplate(TemplateType.Alteration, "alter2");
            library.StartTemplate(TemplateType.Alteration, "alter3");

            library.Find(TemplateType.Alteration, "first").Description.ShouldEqual("the alteration");
            library.Find(TemplateType.Project, "first").Description.ShouldEqual("the solutionProject");
            library.Find(TemplateType.Solution, "first").Description.ShouldEqual("the solution");
        }

        [Test]
        public void can_read_a_template_with_no_description()
        {
            var library = TemplateLibrary.BuildClean("no-description");
            library.StartTemplate(TemplateType.Solution, "simple");

            var template = library.Find(TemplateType.Solution, "simple");
            template.Description.ShouldBeNull();
            template.Name.ShouldEqual("simple");
            template.Type.ShouldEqual(TemplateType.Solution);

        }

        [Test]
        public void can_read_a_template_with_descriptions()
        {
            var library = TemplateLibrary.BuildClean("with-description");
            var builder = library.StartTemplate(TemplateType.Solution, "described");
            builder.WriteDescription("some description");


            var template = library.Find(TemplateType.Solution, "described");
            template.Description.ShouldEqual("some description");
            template.Name.ShouldEqual("described");
            template.Type.ShouldEqual(TemplateType.Solution);

        }
    }
}