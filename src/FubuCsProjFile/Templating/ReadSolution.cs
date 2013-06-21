namespace FubuCsProjFile.Templating
{
    public class ReadSolution : ITemplateStep
    {
        private readonly string _solutionFile;

        public ReadSolution(string solutionFile)
        {
            _solutionFile = solutionFile;
        }

        public void Alter(TemplatePlan plan)
        {
            var solution = Solution.LoadFrom(_solutionFile);
            plan.Solution = solution;
        }

        public string SolutionFile
        {
            get { return _solutionFile; }
        }

        protected bool Equals(ReadSolution other)
        {
            return string.Equals(_solutionFile, other._solutionFile);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReadSolution) obj);
        }

        public override int GetHashCode()
        {
            return (_solutionFile != null ? _solutionFile.GetHashCode() : 0);
        }
    }
}