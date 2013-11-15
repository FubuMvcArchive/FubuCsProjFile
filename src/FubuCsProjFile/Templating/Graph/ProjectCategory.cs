using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FubuCore.Descriptions;

namespace FubuCsProjFile.Templating.Graph
{
    public class ProjectCategory : DescribesItself
    {
        public ProjectCategory()
        {
            Templates = new List<ProjectTemplate>();
        }

        public string Type;
        public readonly IList<ProjectTemplate> Templates;

        public ProjectTemplate FindTemplate(string name)
        {
            return Templates.FirstOrDefault(x => FubuCore.StringExtensions.EqualsIgnoreCase(x.Name, name));
        }

        public void Describe(Description description)
        {
            description.Title = Type + " projects";
            description.ShortDescription = "Project templating options";
            description.AddList("Project Types", Templates);
        }
    }
}