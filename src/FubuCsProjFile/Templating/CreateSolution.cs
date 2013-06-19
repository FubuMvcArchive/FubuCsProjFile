namespace FubuCsProjFile.Templating
{
    public class CreateSolution : ITemplateStep
    {
        private readonly string _solutionName;

        public CreateSolution(string solutionName)
        {
            _solutionName = solutionName;
        }

        public void Alter(TemplatePlan plan)
        {
            // TODO -- needs to check to see if the Solution already exists.  If so, just load it.

            var solution = Solution.CreateNew(plan.SourceDirectory, _solutionName);
            plan.Solution = solution;
        }
    }
}