namespace FubuCsProjFile.Templating
{
    public class CreateSolution : ITemplateStep
    {
        private readonly string _solutionName;

        public CreateSolution(string solutionName)
        {
            _solutionName = solutionName;
        }

        public string SolutionName
        {
            get { return _solutionName; }
        }

        public void Alter(TemplatePlan plan)
        {
            var solution = Solution.CreateNew(plan.SourceDirectory, _solutionName);
            plan.Solution = solution;
        }

        public override string ToString()
        {
            return string.Format("Create solution '{0}'", _solutionName);
        }
    }
}