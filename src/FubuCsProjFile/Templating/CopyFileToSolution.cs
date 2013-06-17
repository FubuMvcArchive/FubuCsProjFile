using FubuCore;

namespace FubuCsProjFile.Templating
{
    // TODO -- have this do a template?
    public class CopyFileToSolution : ITemplateStep
    {
        private readonly string _relativePath;
        private readonly string _source;

        public CopyFileToSolution(string relativePath, string source)
        {
            _relativePath = relativePath.Replace("\\", "/");
            _source = source;
        }

        public void Alter(TemplateContext context)
        {
            var expectedFile = context.Root.AppendPath(_relativePath);
            context.FileSystem.Copy(_source, expectedFile);
        }

        protected bool Equals(CopyFileToSolution other)
        {
            return string.Equals(_relativePath, other._relativePath) && string.Equals(_source.CanonicalPath(), other._source.CanonicalPath());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CopyFileToSolution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_relativePath != null ? _relativePath.GetHashCode() : 0)*397) ^ (_source != null ? _source.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Copy {1} to {0}", _relativePath, _source);
        }
    }
}