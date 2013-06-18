namespace FubuCsProjFile.Templating
{
    public interface ITemplateStep
    {
        void Alter(TemplatePlan plan);
    }
}