using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TemplateContext
    {
        private readonly IFileSystem _fileSystem = new FileSystem();

        public static TemplateContext CreateClean(string directory)
        {
            var system = new FileSystem();
            system.CreateDirectory(directory);
            system.CleanDirectory(directory);

            return new TemplateContext(directory);
        }

        public TemplateContext(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public string RootDirectory { get; set; }
        public Solution Solution { get; set; }

        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>(); 


        public void Add(ITemplateStep step)
        {
            _steps.Add(step);
        }

        public void Execute()
        {
            _steps.Each(x => x.Alter(this));

            if (Solution != null)
            {
                Solution.Save();
            }
        }

        public void AlterFile(string relativeName, Action<List<string>> alter)
        {
            _fileSystem.AlterFlatFile(RootDirectory.AppendPath(relativeName), alter);
        }
    }

    public class ProjectPlan : ITemplateStep
    {
        private readonly string _projectName;
        private readonly IList<IProjectAlteration> _alterations = new List<IProjectAlteration>(); 

        public ProjectPlan(string projectName)
        {
            _projectName = projectName;
        }

        public void Alter(TemplateContext context)
        {
            // TODO -- encapsulate this inside of TemplateContext
            var reference = context.Solution.FindProject(_projectName) ?? context.Solution.AddProject(_projectName);
            _alterations.Each(x => x.Alter(reference.Project));
        }

        public void Add(IProjectAlteration alteration)
        {
            _alterations.Add(alteration);
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public IList<IProjectAlteration> Alterations
        {
            get { return _alterations; }
        }
    }

    public interface ITemplateStep
    {
        void Alter(TemplateContext context);
    }

    public class CopyFileToProject : IProjectAlteration
    {
        public CopyFileToProject(string relativePath, string source)
        {
        }

        public void Alter(CsProjFile file)
        {
            throw new NotImplementedException();
        }
    }

    public class CopyFileToSolution : ITemplateStep
    {
        public CopyFileToSolution(string relativePath, string source)
        {
        }

        public void Alter(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class NugetReference : ITemplateStep
    {
        public string ProjectName { get; set; }
        public string NugetName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class AssemblyReference : IProjectAlteration
    {
        private readonly string _assemblyName;

        public AssemblyReference(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public void Alter(CsProjFile file)
        {
            throw new NotImplementedException();
        }
    }

    public class GemReference : ITemplateStep
    {
        public string GemName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class AssemblyInfoAlteration : IProjectAlteration
    {
        private readonly IEnumerable<string> _additions;

        public AssemblyInfoAlteration(IEnumerable<string> additions)
        {
            _additions = additions;
        }

        public void Alter(CsProjFile file)
        {
            throw new NotImplementedException();
        }
    }



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
    
         * ****AssemblyInfo.cs is combined
         */

        public void ConfigureTree(string directory, TemplateContext context)
        {
            throw new NotImplementedException();
        }

        public void ConfigureProject(string directory, ProjectPlan projectPlan, TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }

    
}