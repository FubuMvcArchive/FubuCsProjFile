using System.IO;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile.ProjectFiles.CsProj
{
    public class CsProjLoader : IProjectLoader
    {
        public bool Matches(string filename)
        {
            return Path.GetExtension(filename).ToLower() == ".csproj";
        }

        public IProjectFile Load(string filename)
        {
            var project = MSBuildProject.LoadFrom(filename);
            return new CsProjFile(filename, project);
        }
    }
}