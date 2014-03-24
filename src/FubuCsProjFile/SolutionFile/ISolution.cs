using System.Collections.Generic;
using FubuCsProjFile.ProjectFiles;

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
        ISolutionProjectFile GetOrAddProject(string projectName);
        ISolutionProjectFile GetOrAddProject(string projectName, ProjectType type);
        ISolutionProjectFile AddProject(IProjectFile project);
        ISolutionProjectFile AddProjectFromTemplate(string projectName, string projectTemplateFile);
        void Save();
        void Save(string path);
        IEnumerable<BuildConfiguration> Configurations();
        GlobalSection FindSection(string name);
    }
}