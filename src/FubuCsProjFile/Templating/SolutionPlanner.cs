using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class SolutionPlanner : TemplatePlanner
    {
        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            SolutionDirectory.PlanForDirectory(directory).Each(plan.Add);
        }
    }
}