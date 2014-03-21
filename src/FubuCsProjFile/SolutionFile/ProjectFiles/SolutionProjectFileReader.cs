using System;
using FubuCore;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFileReader : ISolutionProjectReader
    {
        private SolutionProjectFile _project;

        public SolutionProjectFileReader(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution, IFileSystem fileSystem)
        {
            _project = new SolutionProjectFile(projectType, projectGuid, projectName, relativePath, solution, fileSystem);
            solution.Projects.Add(_project);
        }

        public void Read(string line)
        {
            // NO-OP
        }
    }
}