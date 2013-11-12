using FubuCsProjFile.Templating.Planning;

namespace FubuCsProjFile.Templating.Runtime
{
    public class CreateSolution : ITemplateStep
    {
        private readonly string _solutionName;

        public CreateSolution(string solutionName)
        {
            _solutionName = solutionName;
            Version = Solution.VS2012;
        }

        public string SolutionName
        {
            get { return _solutionName; }
        }

        public string Version { get; set; }

        public void Alter(TemplatePlan plan)
        {
            var solution = Solution.CreateNew(plan.SourceDirectory, _solutionName);
            solution.Version = Version;

            plan.Solution = solution;
        }

        public override string ToString()
        {
            return string.Format("Create solution '{0}'", _solutionName);
        }
    }
}