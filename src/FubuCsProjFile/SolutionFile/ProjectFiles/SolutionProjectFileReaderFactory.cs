using System;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFileReaderFactory : ISolutionProjectSectionReaderFactory
    {
        public bool Matches(Guid type)
        {
            return true;
        }

        public ISolutionProjectReader Build(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            return new SolutionProjectFileReader(projectType, projectGuid, projectName, relativePath, solution);
        }
    }
}