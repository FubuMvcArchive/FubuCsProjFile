using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.MSBuild;
using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.ProjectFiles
{
    public static class ProjectCreator
    {
        private static readonly IDictionary<ProjectType, IPojectCreatorForType> Creators =
            new Dictionary<ProjectType, IPojectCreatorForType>
            {
                {ProjectType.CsProj, new CsProjCreator()}
            };

        public static IProjectFile CreateAtSolutionDirectory(string assemblyName, string directory, ProjectType type)
        {
            return CreateAtSolutionDirectory(assemblyName, directory, type, Guid.NewGuid());
        }

        public static IProjectFile CreateAtSolutionDirectory(string assemblyName, string directory, ProjectType type, Guid projectGuid)
        {
            var fileName = directory.AppendPath(assemblyName).AppendPath(assemblyName) + "." + type.ToString().ToLower();
            return CreateAtLocation(fileName, assemblyName, type, projectGuid);
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName)
        {
            return CreateAtLocation(filename, assemblyName, Guid.NewGuid());
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName, Guid projectGuid)
        {
            return CreateAtLocation(filename, assemblyName, filename.TypeBasedOnFileName(), projectGuid);
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName, ProjectType type)
        {
            return CreateAtLocation(filename, assemblyName, type, Guid.NewGuid());
        }

        public static IProjectFile CreateAtLocation(string filename, string assemblyName, ProjectType type, Guid projectGuid)
        {
            var msBuildProject = Creators[type].CreateMSBuildProject(assemblyName);
            return Create(msBuildProject, filename, projectGuid, type);
        }

        public static IProjectFile Create(MSBuildProject project, string filename, Guid projectGuid)
        {
            return Create(project, filename, projectGuid, filename.TypeBasedOnFileName());
        }

        public static IProjectFile Create(MSBuildProject project, string filename, Guid projectGuid, ProjectType type)
        {
            var group = project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == ProjectFileConstants.PROJECT_GUID))
                ?? project.PropertyGroups.FirstOrDefault()
                ?? project.AddNewPropertyGroup(true);

            @group.SetPropertyValue(ProjectFileConstants.PROJECT_GUID, projectGuid.ToString().ToUpper(), true);

            var file = Creators[type].Create(project, filename);
            file.AssemblyName = file.RootNamespace = file.ProjectName;
            return file;
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