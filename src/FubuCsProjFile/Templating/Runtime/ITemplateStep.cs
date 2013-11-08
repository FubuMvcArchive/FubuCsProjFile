using FubuCsProjFile.Templating.Planning;

namespace FubuCsProjFile.Templating.Runtime
{
    public interface ITemplateStep
    {
        void Alter(TemplatePlan plan);
    }
}