using System;
using System.IO;

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
}