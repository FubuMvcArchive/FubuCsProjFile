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


        // TODO -- do a bulk validation of TemplateRequest against the library 
        public TemplatePlan BuildPlan(TemplateRequest request)
        {
            var plan = new TemplatePlan(request.RootDirectory);
            if (request.SolutionName.IsNotEmpty())
            {
                determineSolutionFileHandling(request, plan);
            }

            applySolutionTemplates(request, plan);
            request.Substitutions.CopyTo(plan.Substitutions);
            
            applyProjectTemplates(request, plan);
            applyTestingTemplates(request, plan);

            return plan;
        }

        private void applyTestingTemplates(TemplateRequest request, TemplatePlan plan)
        {
            request.TestingProjects.Each(proj => {
                buildProjectPlan(plan, proj);
                plan.Add(new CopyProjectReferences(proj.OriginalProject));
            });
        }

        private void applyProjectTemplates(TemplateRequest request, TemplatePlan plan)
        {
            request.Projects.Each(proj => buildProjectPlan(plan, proj));
        }

        private void buildProjectPlan(TemplatePlan plan, ProjectRequest proj)
        {
            var projectPlan = new ProjectPlan(proj.Name);
            plan.Add(projectPlan);

            proj.Substitutions.CopyTo(projectPlan.Substitutions);

            var planner = new ProjectPlanner();
            if (proj.Template.IsNotEmpty())
            {
                planner.CreatePlan(_library.Find(TemplateType.Project, proj.Template), plan);
            }

            _library.Find(TemplateType.Alteration, proj.Alterations)
                    .Each(template => planner.CreatePlan(template, plan));
        }

        private void applySolutionTemplates(TemplateRequest request, TemplatePlan plan)
        {
            var planner = new SolutionPlanner();
            _library.Find(TemplateType.Solution, request.Templates)
                .Each(template => planner.CreatePlan(template, plan));
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