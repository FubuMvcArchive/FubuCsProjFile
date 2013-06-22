namespace FubuCsProjFile.Templating
{
    public interface IProjectAlteration
    {
        void Alter(CsProjFile file, ProjectPlan plan);
    }
}