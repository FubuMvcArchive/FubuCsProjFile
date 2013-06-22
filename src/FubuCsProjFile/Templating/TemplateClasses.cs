using System;
using System.IO;
using FubuCore;
using System.Collections.Generic;

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

//
//            request.TestingProjects.Each(proj => {
//                var testingProject = proj.OriginalProject + ".Testing";
//                var projectPlan = new ProjectPlan(testingProject);
//                plan.Add(projectPlan);
//                plan.Add(new CopyProjectReferences(proj.OriginalProject, testingProject));
//
//                _library.ApplyAll(proj.Templates, plan, ConfigureProjectTemplate);
//            });



            return plan;
        }

        private void applyProjectTemplates(TemplateRequest request, TemplatePlan plan)
        {
            request.Projects.Each(proj => {
                var projectPlan = new ProjectPlan(proj.Name);
                plan.Add(projectPlan);

                var planner = new ProjectPlanner();
                if (proj.Template.IsNotEmpty())
                {
                    planner.CreatePlan(_library.Find(TemplateType.Project, proj.Template).Path, plan);
                }

                _library.Find(TemplateType.Alteration, proj.Alterations)
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

    public class CopyProjectReferences : ITemplateStep
    {
        public CopyProjectReferences(string originalProject, string testProject)
        {
        }

        // TODO -- has to copy all the system assemblies and nuget references of the parent solutionProject
        // TODO -- adds the solutionProject reference to the parent
        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }

    public class SolutionRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Templates { get; set; } // This needs to get a default value
    }

    public class TestProjectRequest
    {
        public string OriginalProject { get; set; }

        public IEnumerable<string> Templates { get; set; } 
    }
}