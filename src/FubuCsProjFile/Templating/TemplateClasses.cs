using System;
using System.IO;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    

    public class TemplatePlanBuilder
    {
        private readonly ITemplateLibrary _library;
        public static readonly SolutionPlanner SolutionPlanner = new SolutionPlanner();
        public static readonly ProjectPlanner Project = new ProjectPlanner();

        public TemplatePlanBuilder(ITemplateLibrary library)
        {
            _library = library;
        }

        public static void ConfigureSolutionTemplate(Template template, TemplatePlan plan)
        {
            SolutionPlanner.CreatePlan(template.Path, plan);
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

//            _library.ApplyAll(request.Templates, plan, ConfigureSolutionTemplate);
//
//            request.Projects.Each(proj => {
//                var projectPlan = new ProjectPlan(proj.Name);
//                plan.Add(projectPlan);
//
//                _library.ApplyAll(proj.Templates, plan, ConfigureProjectTemplate);
//            });
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

        // TODO -- has to copy all the system assemblies and nuget references of the parent project
        // TODO -- adds the project reference to the parent
        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }

    public class TemplateRequest
    {
        public string RootDirectory { get; set; }
        public IEnumerable<string> Templates { get; set; } // at the solution level
        public string SolutionName { get; set; }

        public IEnumerable<ProjectRequest> Projects { get; set; }
        public IEnumerable<TestProjectRequest> TestingProjects { get; set; } 
    }

    public class SolutionRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Templates { get; set; } // This needs to get a default value
    }

    public class ProjectRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Templates { get; set; } 
    }

    public class TestProjectRequest
    {
        public string OriginalProject { get; set; }

        public IEnumerable<string> Templates { get; set; } 
    }

    public interface ITemplateLibrary
    {
        IEnumerable<Template> All();
        Template Find(TemplateType type, string name);
        void ApplyAll(IEnumerable<string> templateNames, TemplatePlan plan, Action<Template, TemplatePlan> action);
    }


    public class RakeFileTransform : ITemplateStep
    {
        public RakeFileTransform(string templateFile)
        {
        }

        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }


    public class SolutionPlanner : TemplatePlanner
    {
        public SolutionPlanner()
        {
            /*
             * TODO
             * copy files and directories to solution
             * create solution
             * 
             * 
             */
        }

        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            SolutionDirectory.PlanForDirectory(directory).Each(plan.Add);
        }
    }


    public class ProjectPlanner : TemplatePlanner
    {
        public static readonly string NugetFile = "nuget.txt";

        public ProjectPlanner()
        {
            Matching(FileSet.Shallow(NugetFile)).Do = (file, plan) => {
                file.ReadLines()
                    .Where(x => x.IsNotEmpty())
                    .Each(line => plan.CurrentProject.NugetDeclarations.Add(line.Trim()));
            };

            /*
             * TODO: 
             * // only looks for a csproj.xml file
             * cs proj files
             * copy files to project directory
             * assembly references
             * assembly info transformer
             * directories
             */
        }

        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            ProjectDirectory.PlanForDirectory(directory).Each(plan.CurrentProject.Add);

        }
    }

}