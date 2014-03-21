using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCsProjFile.MSBuild;
using FubuCsProjFile.ProjectFiles;
using FubuCsProjFile.ProjectFiles.CsProj;
using FubuCsProjFile.SolutionFile.ProjectFiles;

namespace FubuCsProjFile.SolutionFile
{
    public interface ISolution
    {
        IList<ISolutionProject> Projects { get; }
        string Filename { get; }
        string ParentDirectory { get; }
        IList<GlobalSection> Sections { get; }
        string Version { get; set; }
        string Name { get; }
        ISolutionProjectFile FindProject(string projectName);
        // TODO: right now is defaulting to CsProj project type, should we mark this for termination or is that a good default?
        ISolutionProjectFile AddProject(string projectName);
        ISolutionProjectFile AddProject(string projectName, ProjectType type);
        ISolutionProjectFile AddProject(IProjectFile project);
        ISolutionProjectFile AddProjectFromTemplate(string projectName, string projectTemplateFile);
        void Save();
        void Save(string path);
        IEnumerable<BuildConfiguration> Configurations();
        GlobalSection FindSection(string name);
    }

    public class Solution : ISolution
    {
        public IList<ISolutionProject> Projects { get; private set; }
        public string Filename { get; set; }
        public string ParentDirectory { get { return Filename.ParentDirectory(); } }
        public IList<GlobalSection> Sections { get; set; }
        public string Version { get; set; }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(Filename); }
        }

        public Solution(string filename)
        {
            Filename = filename;
            Projects = new List<ISolutionProject>();
            Sections = new List<GlobalSection>();
        }

        public GlobalSection FindSection(string name)
        {
            return Sections.FirstOrDefault(x => x.SectionName == name);
        }

        public ISolutionProjectFile FindProject(string projectName)
        {
            return Projects.OfType<ISolutionProjectFile>().FirstOrDefault(x => x.ProjectName == projectName);
        }

        public ISolutionProjectFile AddProject(string projectName)
        {
            return AddProject(projectName, ProjectType.CsProj);
        }

        public ISolutionProjectFile AddProject(string projectName, ProjectType type)
        {
            var existing = FindProject(projectName);
            if (existing != null)
            {
                return existing;
            }

//            var reference = SolutionProject.CreateNewAt(ParentDirectory, projectName, type);
//            var csProjFile = ProjectCreator.CreateAtSolutionDirectory(projectName, solutionDirectory, type);
//            return new SolutionProject(csProjFile, solutionDirectory);
            var projectFile = ProjectCreator.CreateAtSolutionDirectory(projectName, ParentDirectory, type, Guid.NewGuid());
            var solutionProjectFile = new SolutionProjectFile(projectFile, ParentDirectory);
            Projects.Add(solutionProjectFile);

            return solutionProjectFile;
        }

        public ISolutionProjectFile AddProject(IProjectFile project)
        {
            var existing = FindProject(project.ProjectName);
            if (existing != null)
            {
                return existing;
            }
            
            var reference = new SolutionProjectFile(project, ParentDirectory);
            Projects.Add(reference);
            return reference;
        }

        // TODO: FSharp Support - This is strongly tied to csproj files
        [MarkedForTermination]
        public ISolutionProjectFile AddProjectFromTemplate(string projectName, string projectTemplateFile)
        {
            return AddProjectFromTemplate(projectName, projectTemplateFile, ProjectType.CsProj);
        }

        public ISolutionProjectFile AddProjectFromTemplate(string projectName, string projectTemplateFile, ProjectType type)
        {
            var existing = FindProject(projectName);
            if (existing != null)
            {
                throw new ArgumentOutOfRangeException("projectName", "Project with this name ({0}) already exists in the solution".ToFormat(projectName));
            }

            var filename = ParentDirectory.AppendPath(projectName, projectName + "." + type.ToString().ToLower());

            var msBuildProject = MSBuildProject.CreateFromFile(projectName, projectTemplateFile);
            var project = ProjectCreator.Create(msBuildProject, filename, Guid.NewGuid(), type);

            var reference = new SolutionProjectFile(project, ParentDirectory);
            Projects.Add(reference);

            return reference;
        }

        public void Save()
        {
            Save(Filename);
        }

        public void Save(string filename)
        {
            UpdateProjectConfigurationPlatforms();

            var writer = new StringWriter();

            SolutionFileVersioning.VersionLines[Version ?? SolutionFileVersioning.DefaultVersion].Each(x => writer.WriteLine(x));

            Projects.Each(x => x.ForSolutionFile(writer));

            writer.WriteLine(SolutionConstants.Global);

            Sections.Each(x => x.Write(writer));

            writer.WriteLine(SolutionConstants.EndGlobal);

            new FileSystem().WriteStringToFile(filename, writer.ToString().TrimEnd());

            Projects.OfType<ISolutionProjectFile>().Each(x => x.Save());
        }

        public IEnumerable<BuildConfiguration> Configurations()
        {
            var section = FindSection(SolutionConstants.SolutionConfigurationPlatforms);
            return section == null
                       ? Enumerable.Empty<BuildConfiguration>()
                       : section.Properties.Select(x => new BuildConfiguration(x));
        }

        private void UpdateProjectConfigurationPlatforms()
        {
            var section = FindSection(SolutionConstants.ProjectConfigurationPlatforms);
            if (section == null)
            {
                section = new GlobalSection("GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                Sections.Add(section);
            }

            section.Properties.Clear();
            var configurations = Configurations().ToArray();

            Projects.OfType<ISolutionProjectFile>().Each(proj => {
                configurations.Each(config => config.WriteProjectConfiguration(proj, section));
            });
        }

    }
}