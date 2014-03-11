using FubuCore;
using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.Templating.Runtime
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
            var fileSystem = new FileSystem();
            var rawText = fileSystem.ReadStringFromFile(_source);

            var templatedText = plan.
                ApplySubstitutions(rawText, _relativePath);

            var expectedPath = file.ProjectDirectory.AppendPath(_relativePath);

            fileSystem.WriteStringToFile(expectedPath, templatedText);

            file.Add(new Content(_relativePath));
        }

        public override string ToString()
        {
            return string.Format("Copy {0} to {1}", _source, _relativePath);
        }
    }
}