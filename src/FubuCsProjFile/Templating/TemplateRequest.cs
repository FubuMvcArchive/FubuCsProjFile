using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCsProjFile.Templating
{
    public class TemplateRequest
    {
        private readonly IList<string> _templates = new List<string>();
        private readonly IList<ProjectRequest> _projects = new List<ProjectRequest>();
        private readonly IList<TestProjectRequest> _testingProjects = new List<TestProjectRequest>();
        private readonly Substitutions _substitutions = new Substitutions();

        public IEnumerable<string> Validate()
        {


            throw new NotImplementedException();
        } 

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