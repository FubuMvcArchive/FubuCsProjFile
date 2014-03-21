using System;
using FubuCore;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFileReaderFactory : ISolutionProjectSectionReaderFactory
    {
        private readonly IFileSystem _fileSystem;

        public SolutionProjectFileReaderFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Matches(Guid type)
        {
            return true;
        }

        public ISolutionProjectReader Build(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            return new SolutionProjectFileReader(projectType, projectGuid, projectName, relativePath, solution, _fileSystem);
        }
    }
}