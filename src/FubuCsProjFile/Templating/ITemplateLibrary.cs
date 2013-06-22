using System;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public interface ITemplateLibrary
    {
        IEnumerable<Template> All();
        Template Find(TemplateType type, string name);
        void ApplyAll(IEnumerable<string> templateNames, TemplatePlan plan, Action<Template, TemplatePlan> action);
    }
}