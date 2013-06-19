namespace FubuCsProjFile.Templating
{

    public class GenericPlanner : TemplatePlanner
    {
        public GenericPlanner()
        {


            ShallowMatch(GemReference.File).Do = GemReference.ConfigurePlan;
            ShallowMatch(GitIgnoreStep.File).Do = GitIgnoreStep.ConfigurePlan;

            // TODO -- do the unhandled file copy thing here?
            // TODO -- add the rake transform
        }
    }


}