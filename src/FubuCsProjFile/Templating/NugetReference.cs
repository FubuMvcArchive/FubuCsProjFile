using System;

namespace FubuCsProjFile.Templating
{
    public class NugetReference : ITemplateStep
    {
        public string ProjectName { get; set; }
        public string NugetName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }
}