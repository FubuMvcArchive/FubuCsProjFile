using System.Collections.Generic;

namespace FubuCsProjFile.Templating
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