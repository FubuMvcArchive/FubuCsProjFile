using FubuCsProjFile.Templating;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TemplateRequestTester
    {
        private TemplateLibrary theTemplates;

        [SetUp]
        public void SetUp()
        {
            theTemplates = TemplateLibrary.BuildClean("validation");
            theTemplates.StartTemplate(TemplateType.Solution, "Simple");
            theTemplates.StartTemplate(TemplateType.Solution, "Complex");
            theTemplates.StartTemplate(TemplateType.Project, "MvcApp");
            theTemplates.StartTemplate(TemplateType.Project, "MvcBottle");
            theTemplates.StartTemplate(TemplateType.Alteration, "Assets");
            theTemplates.StartTemplate(TemplateType.Alteration, "HtmlConventions");
        }

        [Test]
        public void validate_when_everything_matches()
        {
            var request = new TemplateRequest();
            request.AddTemplate("Simple");

            request.Validate(theTemplates).Any().ShouldBeFalse();

            request.AddProjectRequest(new ProjectRequest("foo", "MvcApp"));

            request.Validate(theTemplates).Any().ShouldBeFalse();

            request.Projects.Single().AddAlteration("Assets");

            request.AddTestingRequest(new TestProjectRequest("foo", "MvcApp", "original"));

            request.Validate(theTemplates).Any().ShouldBeFalse();


        }

        [Test]
        public void validate_missing_solution_template()
        {
            var request = new TemplateRequest();
            request.AddTemplate("NonExistent");

            var missing = request.Validate(theTemplates).Single();
            missing.Name.ShouldEqual("NonExistent");
            missing.TemplateType.ShouldEqual(TemplateType.Solution);
            missing.ValidChoices.ShouldHaveTheSameElementsAs("Complex", "Simple");
        }

        [Test]
        public void validate_missing_project_template()
        {
            var request = new TemplateRequest();
            request.AddTemplate("Simple");
            request.AddProjectRequest(new ProjectRequest("foo", "NonExistent"));

            var missing = request.Validate(theTemplates).Single();
            missing.Name.ShouldEqual("NonExistent");
            missing.TemplateType.ShouldEqual(TemplateType.Project);
            missing.ValidChoices.ShouldHaveTheSameElementsAs("MvcApp", "MvcBottle");
        }

        [Test]
        public void validate_missing_alteration_template()
        {
            var request = new TemplateRequest();
            request.AddTemplate("Simple");
            request.AddProjectRequest(new ProjectRequest("foo", "MvcApp" ));
            request.Projects.Last().AddAlteration("NonExistent");

            var missing = request.Validate(theTemplates).Single();
            missing.Name.ShouldEqual("NonExistent");
            missing.TemplateType.ShouldEqual(TemplateType.Alteration);
            missing.ValidChoices.ShouldHaveTheSameElementsAs("Assets", "HtmlConventions");
        }

        [Test]
        public void validate_missing_testing_template()
        {
            var request = new TemplateRequest();
            request.AddTemplate("Simple");
            request.AddProjectRequest(new ProjectRequest("foo", "MvcApp"));
            request.AddTestingRequest(new TestProjectRequest("foo", "NonExistent", "original"));

            var missing = request.Validate(theTemplates).Single();
            missing.Name.ShouldEqual("NonExistent");
            missing.TemplateType.ShouldEqual(TemplateType.Project);
            missing.ValidChoices.ShouldHaveTheSameElementsAs("MvcApp", "MvcBottle");
        }

    }
}