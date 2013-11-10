using System.Collections.Generic;
using System.Linq;

namespace FubuCsProjFile.Templating.Graph
{
    public class ProjectCategory
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
    }
}