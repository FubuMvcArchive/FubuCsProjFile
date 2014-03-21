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
            var project = new SolutionProjectFile(projectType, projectGuid, projectName, relativePath, solution);
            solution.Projects.Add(project);

            return new NoOpSolutionProjectReader();
        }
    }
}