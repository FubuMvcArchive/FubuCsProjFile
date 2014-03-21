using System;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFileReader : ISolutionProjectReader
    {
        private SolutionProjectFile _project;

        public SolutionProjectFileReader(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            _project = new SolutionProjectFile(projectType, projectGuid, projectName, relativePath, solution);
            solution.Projects.Add(_project);
        }

        public void Read(string line)
        {
            // NO-OP
        }
    }
}