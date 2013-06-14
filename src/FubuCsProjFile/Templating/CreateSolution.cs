namespace FubuCsProjFile.Templating
{
    public class CreateSolution : ITemplateStep
    {
        private readonly string _solutionName;

        public CreateSolution(string solutionName)
        {
            _solutionName = solutionName;
        }

        public void Alter(TemplateContext context)
        {
            var solution = Solution.CreateNew(context.SourceDirectory, _solutionName);
            context.Solution = solution;
        }
    }
}