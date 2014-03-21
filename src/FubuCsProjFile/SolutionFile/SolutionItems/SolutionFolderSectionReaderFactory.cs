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
            var folder = new SolutionFolder(projectGuid, projectName, relativePath);
            solution.Projects.Add(folder);

            return new SolutionFolderSectionReader(folder);
        }
    }
}