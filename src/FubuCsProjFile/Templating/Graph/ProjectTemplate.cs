using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating.Graph
{
    public class ProjectTemplate
    {
        public ProjectTemplate()
        {
            Options = new List<Option>();
            Selections = new List<OptionSelection>();
        }

        // If this is null, it's not a "new" project
        public string Template;

        public IList<string> Alterations = new List<string>();
        public string Description;
        public string Name;

        public IList<Option> Options;
        public IList<OptionSelection> Selections;

        public Option FindOption(string optionName)
        {
            return Options.FirstOrDefault(x => x.Name.EqualsIgnoreCase(optionName));
        }

        public ProjectRequest BuildProjectRequest(TemplateChoices choices)
        {
            var request = new ProjectRequest(choices.ProjectName, Template);
            request.Alterations.AddRange(Alterations);

            if (choices.Options != null)
            {
                choices.Options.Each(o =>
                {
                    var opt = FindOption(o);
                    if (opt == null) throw new Exception("Unknown option '{0}' for project type {1}".ToFormat(o, Name));

                    request.Alterations.AddRange(opt.Alterations);
                });
            }

            if (Selections != null)
            {
                Selections.Each(selection => selection.Configure(choices, request));
            }

            choices.Inputs.Each((key, value) => request.Substitutions.Set(key, value));

            return request;
        }
    }
}