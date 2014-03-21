using System;
using System.IO;
using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.SolutionFile
{
    public interface ISolutionProject
    {
        Guid Type { get; }
        Guid ProjectGuid { get; }
        string ProjectName { get; }
        string RelativePath { get; }
        void ForSolutionFile(StringWriter writer);
    }

    public interface ISolutionProjectFile : ISolutionProject
    {
        IProjectFile Project { get; } 
        void Save();
    }
}