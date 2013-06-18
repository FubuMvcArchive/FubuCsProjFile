namespace FubuCsProjFile.Templating
{

    public class GenericPlanner : TemplatePlanner
    {
        public GenericPlanner()
        {


            ShallowMatch(GemReference.File).Do = GemReference.ConfigurePlan;
            ShallowMatch(GitIgnoreStep.File).Do = GitIgnoreStep.ConfigurePlan;

            // TODO -- do the unhandled file copy thing here?
        }
    }

    public class SolutionPlanner : TemplatePlanner
    {
        public SolutionPlanner()
        {
            /*
             * TODO
             * copy files and directories to solution
             * create solution
             * 
             * 
             */
        }
    }

    public class ProjectCreationPlanner : TemplatePlanner
    {
        public ProjectCreationPlanner()
        {
            // only looks for a csproj.xml file
        }
    }

    public class ProjectAlterationPlanner : TemplatePlanner
    {
        public ProjectAlterationPlanner()
        {
            /*
             * TODO:  
             * cs proj files
             * copy files to project directory
             * nuget references
             * assembly references
             * assembly info transformer
             * directories
             */
        }
    }

}