using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class CopyFileToSolution : ITemplateStep
    {
        private readonly string _relativePath;
        private readonly string _source;

        public CopyFileToSolution(string relativePath, string source)
        {
            _relativePath = relativePath;
            _source = source;
        }

        public void Alter(TemplateContext context)
        {
            var expectedFile = context.RootDirectory.AppendPath(_relativePath);
            context.FileSystem.Copy(_source, expectedFile);
        }
    }
}