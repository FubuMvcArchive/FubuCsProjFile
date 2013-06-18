namespace FubuCsProjFile.Templating
{

    public class GenericPlanner : TemplatePlanner
    {
        public GenericPlanner()
        {
            ShallowMatch(GemReference.File).Do = GemReference.ConfigurePlan;
            ShallowMatch(GitIgnoreStep.File).Do = GitIgnoreStep.ConfigurePlan;
        }
    }
}