using System;
using System.Collections.Generic;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile.Templating
{
    public interface ITemplateLibrary
    {
        IEnumerable<Template> All();
        Template Find(TemplateType type, string name);

        IEnumerable<Template> Find(TemplateType type, IEnumerable<string> names);

        IEnumerable<MissingTemplate> Validate(TemplateType type, params string[] names);
    }

    public class MissingTemplate
    {
        public string Name { get; set; }
        public TemplateType TemplateType { get; set; }
        public string[] ValidChoices { get; set; }

        public override string ToString()
        {
            var validChoiceString = ValidChoices.Select(x => "'{0}'".ToFormat(x)).Join(", ");
            return "Unknown {0} template '{1}', valid choices are {2}"
                .ToFormat(TemplateType.ToString(), Name, validChoiceString);

        }
    }
}