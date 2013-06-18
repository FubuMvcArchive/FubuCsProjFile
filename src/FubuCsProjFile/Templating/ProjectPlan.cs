using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class ProjectPlan : ITemplateStep
    {
        private readonly string _projectName;
        private readonly IList<IProjectAlteration> _alterations = new List<IProjectAlteration>(); 

        public ProjectPlan(string projectName)
        {
            _projectName = projectName;
        }

        public void Alter(TemplatePlan plan)
        {
            // TODO -- encapsulate this inside of TemplatePlan
            var reference = plan.Solution.FindProject(_projectName) ?? plan.Solution.AddProject(_projectName);
            _alterations.Each(x => x.Alter(reference.Project));
        }

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
    }
}