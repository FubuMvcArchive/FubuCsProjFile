using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile.ProjectFiles
{
    public interface IPojectCreatorForType
    {
        IProjectFile Create(MSBuildProject project, string filename);
        MSBuildProject CreateMSBuildProject(string assemblyName);
    }
}