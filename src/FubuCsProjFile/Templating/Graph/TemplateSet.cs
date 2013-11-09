using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating.Graph
{
    public class TemplateSet
    {
        // If this is null, it's not a "new" project
        public string Template;
        public IList<string> Tags = new List<string>();

        public IList<string> Alterations = new List<string>();
        public string Description;
        public string Name;

        public IList<Option> Options = new List<Option>();
        public IList<OptionSelection> Selections = new List<OptionSelection>();

        public bool MatchesTag(string tag)
        {
            return Tags.Any(t => FubuCore.StringExtensions.EqualsIgnoreCase(t, tag));
        }

        public Option FindOption(string optionName)
        {
            return Options.FirstOrDefault(x => x.Name.EqualsIgnoreCase(optionName));
        }
    }
}