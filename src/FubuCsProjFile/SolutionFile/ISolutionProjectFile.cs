using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.SolutionFile
{
    public interface ISolutionProjectFile : ISolutionProject
    {
        IProjectFile Project { get; } 
        void Save();
    }
}