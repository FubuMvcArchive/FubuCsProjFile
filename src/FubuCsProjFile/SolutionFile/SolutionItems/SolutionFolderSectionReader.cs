using System;

namespace FubuCsProjFile.SolutionFile.SolutionItems
{
    public class SolutionFolderSectionReader : ISolutionProjectReader
    {
        private readonly SolutionFolder _folder;

        public SolutionFolderSectionReader(Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            _folder = new SolutionFolder(projectGuid, projectName, relativePath);
            solution.Projects.Add(_folder);
        }

        public void Read(string line)
        {
            _folder.RawLines.Add(line);
        }
    }
}