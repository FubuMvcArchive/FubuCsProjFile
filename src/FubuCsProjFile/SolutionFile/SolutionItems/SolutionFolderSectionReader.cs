namespace FubuCsProjFile.SolutionFile.SolutionItems
{
    public class SolutionFolderSectionReader : ISolutionProjectReader
    {
        private readonly SolutionFolder _folder;

        public SolutionFolderSectionReader(SolutionFolder folder)
        {
            _folder = folder;
        }

        public void Read(string line)
        {
            _folder.RawLines.Add(line);
        }
    }
}