using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCsProjFile.Templating.Graph
{
    public class TemplateRequest
    {
        private readonly IList<string> _templates = new List<string>();
        private readonly IList<ProjectRequest> _projects = new List<ProjectRequest>();
        private readonly IList<ProjectRequest> _testingProjects = new List<ProjectRequest>();
        private readonly Substitutions _substitutions = new Substitutions();

        public Substitutions Substitutions
        {
            get { return _substitutions; }
        }

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
        public string Version { get; set; }

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

        public void AddProjectRequest(string name, string template, Action<ProjectRequest> configuration = null)
        {
            var request = new ProjectRequest(name, template);
            if (configuration != null) configuration(request);

            _projects.Add(request);
        }

        public IEnumerable<ProjectRequest> TestingProjects
        {
            get { return _testingProjects; }
            set
            {
                _testingProjects.Clear();
                _testingProjects.AddRange(value);
            }
        }

        public void AddTestingRequest(ProjectRequest request)
        {
            _testingProjects.Add(request);
        }

        public IEnumerable<MissingTemplate> Validate(ITemplateLibrary templates)
        {
            var solutionErrors = templates.Validate(TemplateType.Solution, _templates.ToArray());
            var projectErrors = templates.Validate(TemplateType.Project, _projects.Select(x => x.Template).ToArray());
            var alterationErrors = templates.Validate(TemplateType.Alteration,
                                                      _projects.SelectMany(x => x.Alterations).ToArray());

            var testingErrors = templates.Validate(TemplateType.Project,
                                                   _testingProjects.Select(x => x.Template).ToArray());

            var testingAlterationErrors = templates.Validate(TemplateType.Alteration,
                                                             _testingProjects.SelectMany(x => x.Alterations).ToArray());



            return solutionErrors
                .Union(projectErrors)
                .Union(alterationErrors)
                .Union(testingErrors)
                .Union(testingAlterationErrors);

        } 

    }
}