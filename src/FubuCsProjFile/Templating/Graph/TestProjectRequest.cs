namespace FubuCsProjFile.Templating.Graph
{
    public class TestProjectRequest : ProjectRequest
    {
        public TestProjectRequest(string name, string template, string originalProject) : base(name, template)
        {
            OriginalProject = originalProject;
        }

        public string OriginalProject { get; private set; }
    }
}