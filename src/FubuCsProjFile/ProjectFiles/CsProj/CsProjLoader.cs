using System.IO;
using FubuCore;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile.ProjectFiles.CsProj
{
    public class CsProjLoader : IProjectLoader
    {
        private readonly IFileSystem _fileSystem;

        public CsProjLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Matches(string filename)
        {
            return Path.GetExtension(filename).ToLower() == ".csproj";
        }

        public IProjectFile Load(string filename)
        {
            if (!_fileSystem.FileExists(filename))
            {
                throw new FileNotFoundException("File does not exist", "filename");
            }

            var project = MSBuildProject.LoadFrom(filename);
            return new CsProjFile(filename, project);
        }
    }
}