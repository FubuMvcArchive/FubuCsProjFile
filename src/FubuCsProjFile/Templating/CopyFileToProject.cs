namespace FubuCsProjFile.Templating
{
    public class CopyFileToProject : IProjectAlteration
    {
        private readonly string _relativePath;
        private readonly string _source;

        public CopyFileToProject(string relativePath, string source)
        {
            _relativePath = relativePath.Replace('\\', '/');
            _source = source;
        }

        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            file.CopyFileTo(_source, _relativePath);
        }
    }
}