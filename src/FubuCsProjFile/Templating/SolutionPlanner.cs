using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class SolutionPlanner : TemplatePlanner
    {
        public SolutionPlanner()
        {
            ShallowMatch(Substitutions.ConfigFile).Do = (file, plan) =>
            {
                plan.Substitutions.ReadFrom(file.Path);
            };

            ShallowMatch(Input.File).Do = (file, plan) => {
                var inputs = Input.ReadFromFile(file.Path);
                plan.Substitutions.ReadInputs(inputs, plan.MissingInputs.Add);
            };

            ShallowMatch(TemplatePlan.InstructionsFile).Do = (file, plan) =>
            {
                var instructions = file.ReadAll();
                plan.AddInstructions(instructions);
            };
        }

        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            SolutionDirectory.PlanForDirectory(directory).Each(plan.Add);
        }
    }
}