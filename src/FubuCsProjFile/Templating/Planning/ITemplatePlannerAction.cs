using System;

namespace FubuCsProjFile.Templating.Planning
{
    public interface ITemplatePlannerAction
    {
        Action<TextFile, TemplatePlan> Do { set; }
    }
}