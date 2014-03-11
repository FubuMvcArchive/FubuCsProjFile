using System;
using System.Linq;
using FubuCore;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile.ProjectFiles.CsProj
{
    public class CsProjCreator : IPojectCreatorForType
    {
        public IProjectFile CreateAtSolutionDirectory(string assemblyName, string directory)
        {
            var fileName = directory.AppendPath(assemblyName).AppendPath(assemblyName) + ".csproj";
            var project = MSBuildProject.Create<CsProjFile>(assemblyName);
            return CreateCore(project, fileName);
        }

        public IProjectFile CreateAtLocation(string filename, string assemblyName)
        {
            return CreateCore(MSBuildProject.Create<CsProjFile>(assemblyName), filename);
        }

        private IProjectFile CreateCore(MSBuildProject project, string fileName)
        {
            var group = project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == CsProjFile.PROJECTGUID)) ??
                        project.PropertyGroups.FirstOrDefault() ?? project.AddNewPropertyGroup(true);

            @group.SetPropertyValue(CsProjFile.PROJECTGUID, Guid.NewGuid().ToString().ToUpper(), true);

            var file = new CsProjFile(fileName, project);
            file.AssemblyName = file.RootNamespace = file.ProjectName;
            return file;
        }
    }
}
