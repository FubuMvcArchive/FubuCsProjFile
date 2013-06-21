using System;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    

    public class TemplatePlanBuilder
    {
        private readonly ITemplateLibrary _library;
        public static readonly SolutionPlanner SolutionPlanner = new SolutionPlanner();
        public static readonly GenericPlanner GenericPlanner = new GenericPlanner();
        public static readonly ProjectPlanner Project = new ProjectPlanner();

        public TemplatePlanBuilder(ITemplateLibrary library)
        {
            _library = library;
        }

        public static void ConfigureSolutionTemplate(Template template, TemplatePlan plan)
        {
            SolutionPlanner.CreatePlan(template.Path, plan);
            GenericPlanner.CreatePlan(template.Path, plan);

            plan.CopyUnhandledFiles(template.Path);
        }

        public static void ConfigureProjectTemplate(Template template, TemplatePlan plan)
        {
            new ProjectPlanner().CreatePlan(template.Path, plan);
            new GenericPlanner().CreatePlan(template.Path, plan);

            // TODO -- copy other files and directories
        }


        // TODO -- do a bulk validation of TemplateRequest against the library 
        public TemplatePlan BuildPlan(TemplateRequest request)
        {
            var plan = new TemplatePlan(request.RootDirectory);
            
            if (request.SolutionName.IsNotEmpty())
            {
                plan.Add(new CreateSolution(request.SolutionName));
            }

            _library.ApplyAll(request.Templates, plan, ConfigureSolutionTemplate);

            request.Projects.Each(proj => {
                var projectPlan = new ProjectPlan(proj.Name);
                plan.Add(projectPlan);

                _library.ApplyAll(proj.Templates, plan, ConfigureProjectTemplate);
            });

            request.TestingProjects.Each(proj => {
                var testingProject = proj.OriginalProject + ".Testing";
                var projectPlan = new ProjectPlan(testingProject);
                plan.Add(projectPlan);
                plan.Add(new CopyProjectReferences(proj.OriginalProject, testingProject));

                _library.ApplyAll(proj.Templates, plan, ConfigureProjectTemplate);
            });



            return plan;
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

    public class NugetReference : ITemplateStep
    {
        public string ProjectName { get; set; }
        public string NugetName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectDirectory : IProjectAlteration
    {
        private readonly string _relativePath;

        public ProjectDirectory(string relativePath)
        {
            _relativePath = relativePath;
        }

        public void Alter(CsProjFile file)
        {
            throw new NotImplementedException();
        }
    }

    public class SolutionDirectory : ITemplateStep
    {
        private readonly string _relativePath;

        public SolutionDirectory(string relativePath)
        {
            _relativePath = relativePath;
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }

        protected bool Equals(SolutionDirectory other)
        {
            return string.Equals(_relativePath, other._relativePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolutionDirectory) obj);
        }

        public override int GetHashCode()
        {
            return (_relativePath != null ? _relativePath.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Create solution directory: {0}", _relativePath);
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
    }


    public class ProjectPlanner : TemplatePlanner
    {
        public ProjectPlanner()
        {
            /*
             * TODO: 
             * // only looks for a csproj.xml file
             * cs proj files
             * copy files to project directory
             * nuget references
             * assembly references
             * assembly info transformer
             * directories
             */
        }
    }

}