namespace FubuCsProjFile.Templating
{
    public interface ITemplateStep
    {
        void Alter(TemplateContext context);
    }
}