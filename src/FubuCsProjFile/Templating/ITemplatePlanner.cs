namespace FubuCsProjFile.Templating
{
    public interface ITemplatePlanner
    {
        void DetermineSteps(string directory, TemplatePlan plan);
    }
}