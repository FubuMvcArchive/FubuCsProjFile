namespace FubuCsProjFile.ProjectFiles
{
    public interface IProjectLoader
    {
        bool Matches(string filename);
        IProjectFile Load(string filename);
    }
}