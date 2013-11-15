using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuCsProjFile.Templating.Graph
{
    public class OptionSelection : DescribesItself
    {
        public string Description;
        public string Name;
        public IList<Option> Options = new List<Option>();

        public void Configure(TemplateChoices choices, ProjectRequest request)
        {
            choices.Options = choices.Options ?? new string[0];

            var option = choices.Selections.Has(Name)
                ? FindOption(choices.Selections[Name])
                : Options.FirstOrDefault(x => choices.Options.Any(o => o.EqualsIgnoreCase(x.Name))) ?? Options.First();

            request.Alterations.AddRange(option.Alterations);
        }

        public Option FindOption(string name)
        {
            return Options.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public void Describe(Description description)
        {
            description.Title = Name;
            description.ShortDescription = Description + " (default is '{0}')".ToFormat(Options.First().Name);
            description.AddList("Options", Options);
        }
    }
}