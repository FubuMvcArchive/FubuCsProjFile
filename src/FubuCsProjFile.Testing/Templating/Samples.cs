using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Planning;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating
{
    [Explicit]
    public class Samples
    {
        public void generate_a_simple_template()
        {
            // SAMPLE: generating-with-templates
            var library = new TemplateLibrary("path to your templates");
            var builder = new TemplatePlanBuilder(library);
            var request = buildTemplateRequest();

            // Build a TemplatePlan
            var plan = builder.BuildPlan(request);
            plan.Execute();
            // ENDSAMPLE
        }

        // SAMPLE: building-template-request
        private static TemplateRequest buildTemplateRequest()
        {
            var request = new TemplateRequest
            {
                RootDirectory = "root directory",
                SolutionName = "MySolution.sln"
            };

            // Add a project named "MyProject" using a project template named "basic-new-project"
            request.AddProjectRequest("MyProject", "basic-new-project", project => {
                // Add some extra templates from the /alterations folder
                project.Alterations.Add("raven");
                project.Alterations.Add("spark");

                // Add some substitution values for the templating if desired
                // See the topic page for subsitutions and inputs
                project.Substitutions.Set("%SHORT_NAME%", "MyProj");
            });

            // Add a testing project related to our first project using the "unit-testing" project template
            // See the topic about testing projects
            request.AddTestingRequest(new ProjectRequest("MyProject.Testing", "unit-testing", "MyProject"));

            return request;
        }
        // ENDSAMPLE
    }
}