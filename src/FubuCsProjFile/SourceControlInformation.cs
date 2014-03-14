namespace FubuCsProjFile
{
    public class SourceControlInformation
    {
        public SourceControlInformation(string projectUniqueName, string projectName, string projectLocalPath)
        {
            this.ProjectUniqueName = projectUniqueName;
            this.ProjectName = projectName;
            this.ProjectLocalPath = projectLocalPath;
        }

        public string ProjectUniqueName { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectLocalPath { get; private set; }
    }
}