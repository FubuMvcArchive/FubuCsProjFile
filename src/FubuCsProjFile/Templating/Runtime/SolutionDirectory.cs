using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating.Planning;

namespace FubuCsProjFile.Templating.Runtime
{
    public class SolutionDirectory : ITemplateStep
    {
        private readonly string _relativePath;

        public static IEnumerable<SolutionDirectory> PlanForDirectory(string root)
        {
            return Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
                            .Select(dir => new SolutionDirectory(dir.PathRelativeTo(root)));
        }

        public SolutionDirectory(string relativePath)
        {
            _relativePath = relativePath.Replace("\\", "/");
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void Alter(TemplatePlan plan)
        {
            new FileSystem().CreateDirectory(plan.Root, _relativePath);
        }

        protected bool Equals(SolutionDirectory other)
        {
            return string.Equals(_relativePath, other._relativePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolutionDirectory) obj);
        }

        public override int GetHashCode()
        {
            return (_relativePath != null ? _relativePath.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Create solution directory: {0}", _relativePath);
        }
    }
}