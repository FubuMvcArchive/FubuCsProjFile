using System;
using System.Collections.Generic;
using System.IO;
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

        public static IProjectFile CreateAtLocation(string filename, string assemblyName)
        {
            return CreateAtLocation(filename, assemblyName, filename.TypeBasedOnFileName());
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName, ProjectType type)
        {
            return Creators[type].CreateAtLocation(filename, assemblyName);
        }

        public static ProjectType TypeBasedOnFileName(this string filename)
        {
            var extension = Path.GetExtension(filename).Trim('.').ToLower();

            switch (extension)
            {
                case "csproj":
                    return ProjectType.CsProj;
                case "fsproj":
                    return ProjectType.FsProj;
                default:
                    throw new ArgumentException("filename is not a valid visual studio project extension type", "filename");
            }
        }
    }
}