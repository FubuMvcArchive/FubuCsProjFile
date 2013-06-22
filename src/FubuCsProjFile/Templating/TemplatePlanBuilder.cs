using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TemplatePlanBuilder
    {
        private readonly ITemplateLibrary _library;

        public TemplatePlanBuilder(ITemplateLibrary library)
        {
            _library = library;
        }


        public static void ConfigureProjectTemplate(Template template, TemplatePlan plan)
        {
            new ProjectPlanner().CreatePlan(template.Path, plan);
        }


        // TODO -- do a bulk validation of TemplateRequest against the library 
        public TemplatePlan BuildPlan(TemplateRequest request)
        {
            var plan = new TemplatePlan(request.RootDirectory);
            
            if (request.SolutionName.IsNotEmpty())
            {
                determineSolutionFileHandling(request, plan);
            }

            applySolutionTemplates(request, plan);
            applyProjectTemplates(request, plan);

            return plan;
        }

        private void applyProjectTemplates(TemplateRequest request, TemplatePlan plan)
        {
            request.Projects.Each(proj => {
                var projectPlan = new ProjectPlan(proj.Name);
                plan.Add(projectPlan);

                var planner = new ProjectPlanner();
                if (FubuCore.StringExtensions.IsNotEmpty(proj.Template))
                {
                    planner.CreatePlan(_library.Find((TemplateType) TemplateType.Project, (string) proj.Template).Path, plan);
                }

                _library.Find((TemplateType) TemplateType.Alteration, (IEnumerable<string>) proj.Alterations)
                        .Each(template => { planner.CreatePlan(template.Path, plan); });
            });
        }

        private void applySolutionTemplates(TemplateRequest request, TemplatePlan plan)
        {
            var planner = new SolutionPlanner();
            _library.Find(TemplateType.Solution, request.Templates).Each(template =>
            {
                planner.CreatePlan(template.Path, plan);
            });
        }

        private static void determineSolutionFileHandling(TemplateRequest request, TemplatePlan plan)
        {
            var sourceDirectory = plan.SourceDirectory;
            var expectedFile = sourceDirectory.AppendPath(request.SolutionName);
            if (Path.GetExtension(expectedFile) != ".sln")
            {
                expectedFile += ".sln";
            }

            if (File.Exists(expectedFile))
            {
                plan.Add(new ReadSolution(expectedFile));
            }
            else
            {
                plan.Add(new CreateSolution(request.SolutionName));
            }
        }
    }
}