using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class TemplateRequest
    {
        private readonly IList<string> _templates = new List<string>();
        private readonly IList<ProjectRequest> _projects = new List<ProjectRequest>();
        private readonly IList<TestProjectRequest> _testingProjects = new List<TestProjectRequest>();

        public string RootDirectory { get; set; }
        public IEnumerable<string> Templates
        {
            get { return _templates; }
            set
            {
                _templates.Clear();
                _templates.AddRange(value);
            }
        }

        public void AddTemplate(string template)
        {
            _templates.Add(template);
        }

        // at the solution level
        public string SolutionName { get; set; }

        public IEnumerable<ProjectRequest> Projects
        {
            get { return _projects; }
            set
            {
                _projects.Clear();
                _projects.AddRange(value);
            }
        }

        public void AddProjectRequest(ProjectRequest request)
        {
            _projects.Add(request);
        }

        public IEnumerable<TestProjectRequest> TestingProjects
        {
            get { return _testingProjects; }
            set
            {
                _testingProjects.Clear();
                _testingProjects.AddRange(value);
            }
        }

        public void AddTestingRequest(TestProjectRequest request)
        {
            _testingProjects.Add(request);
        }
    }
}