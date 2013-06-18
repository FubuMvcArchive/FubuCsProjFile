using System;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class TemplateBuilder
    {


        /*
         * .cs files are CodeFileTemplate
         * description.txt is ignored
         * ignore.txt gets written to .gitignore in the solution
         * nuget.txt gets added to ripple and/or nuget dependencies
         * gems.txt gets added to Gemfile
         * files we don't recognize just get copied
         * references.txt <-- adds system assembly references
         * csproj.xml -- csproj
    
         * ****AssemblyInfo.cs is combined
         */

        public void ConfigureTree(string directory, TemplatePlan plan)
        {
            new GenericPlanner().CreatePlan(directory, plan);

            // Need to do something similar for projects
            plan.CopyUnhandledFilesToRoot(directory);
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