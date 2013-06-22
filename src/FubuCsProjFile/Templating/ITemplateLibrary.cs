using System;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public interface ITemplateLibrary
    {
        IEnumerable<Template> All();
        Template Find(TemplateType type, string name);

        IEnumerable<Template> Find(TemplateType type, IEnumerable<string> names);
    }
}