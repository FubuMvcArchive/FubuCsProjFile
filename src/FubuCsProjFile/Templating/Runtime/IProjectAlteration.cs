using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.Templating.Runtime
{
    public interface IProjectAlteration
    {
        void Alter(IProjectFile file, ProjectPlan plan);
    }
}