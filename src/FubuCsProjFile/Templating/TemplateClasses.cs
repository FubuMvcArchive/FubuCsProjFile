using System;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{

    // This is going to give you access to what templates exist, what category they are,
    // and how to access them
    public interface ITemplateLibrary
    {
        IEnumerable<Template> All();
        Template Find(string name);
    }

    public class Template
    {
        public TemplateType Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }

    public enum TemplateType
    {
        Solution,
        //SolutionAlteration, ?  what would we use this for?  nuspec(?)  Easier to do that via a project maybe.
        Project,
        ProjectAlteration,
        UnitTest
    }


    public class TemplateBuilder
    {
        public void ConfigureTree(string directory, TemplatePlan plan)
        {
            new GenericPlanner().CreatePlan(directory, plan);

            // Need to do something similar for projects
            plan.CopyUnhandledFiles(directory);
        }

        public void ConfigureProject(string directory, ProjectPlan projectPlan, TemplatePlan plan)
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
}