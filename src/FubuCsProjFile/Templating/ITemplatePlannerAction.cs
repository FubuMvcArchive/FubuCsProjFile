using System;

namespace FubuCsProjFile.Templating
{
    public interface ITemplatePlannerAction
    {
        Action<TextFile, TemplatePlan> Do { set; }
    }
}