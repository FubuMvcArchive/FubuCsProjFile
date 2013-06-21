using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class ProjectPlan : ITemplateStep
    {
        private readonly string _projectName;
        private readonly IList<IProjectAlteration> _alterations = new List<IProjectAlteration>(); 
        private readonly IList<string> _nugetDeclarations = new List<string>(); 

        public ProjectPlan(string projectName)
        {
            _projectName = projectName;
        }

        public void Alter(TemplatePlan plan)
        {
            var reference = plan.Solution.FindProject(_projectName);
            if (reference == null)
            {
                reference = ProjectTemplateFile.IsEmpty()
                                ? plan.Solution.AddProject(_projectName)
                                : plan.Solution.AddProjectFromTemplate(_projectName, ProjectTemplateFile);
            }

            _alterations.Each(x => x.Alter(reference.Project));
        }

        public IList<string> NugetDeclarations
        {
            get { return _nugetDeclarations; }
        }

        // TODO -- do something with this in Alter.  Might need to blow up if the project already exists
        public string ProjectTemplateFile { get; set; }

        public void Add(IProjectAlteration alteration)
        {
            _alterations.Add(alteration);
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public IList<IProjectAlteration> Alterations
        {
            get { return _alterations; }
        }

        public string ToNugetImportStatement()
        {
            return "{0}: {1}".ToFormat(ProjectName, _nugetDeclarations.Join(", "));
        }
    }
}