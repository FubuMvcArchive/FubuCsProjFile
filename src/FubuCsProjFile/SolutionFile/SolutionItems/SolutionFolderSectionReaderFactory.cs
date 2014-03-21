using System;

namespace FubuCsProjFile.SolutionFile.SolutionItems
{
    public class SolutionFolderSectionReaderFactory : ISolutionProjectSectionReaderFactory
    {
        public bool Matches(Guid type)
        {
            return SolutionFolder.TypeId == type;
        }

        public ISolutionProjectReader Build(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            return new SolutionFolderSectionReader(projectGuid, projectName, relativePath, solution);
        }
    }
}