namespace FubuCsProjFile.ProjectFiles
{
    public interface IPojectCreatorForType
    {
        IProjectFile CreateAtSolutionDirectory(string assemblyName, string directory);
        IProjectFile CreateAtLocation(string filename, string assemblyName);
    }
}