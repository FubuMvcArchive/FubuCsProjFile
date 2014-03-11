using System.Collections.Generic;
using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.ProjectFiles
{
    public static class ProjectCreator
    {
        private static readonly IDictionary<ProjectType, IPojectCreatorForType> Creators = new Dictionary<ProjectType, IPojectCreatorForType>
        {
            {ProjectType.CsProj, new CsProjCreator()}
        }; 

        public static IProjectFile CreateAtSolutionDirectory(string assemblyName, string directory, ProjectType type)
        {
            return Creators[type].CreateAtSolutionDirectory(assemblyName, directory);
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName, ProjectType type)
        {
            return Creators[type].CreateAtLocation(filename, assemblyName);
        }
    }
}