using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.ProjectFiles.CsProj;
using FubuCsProjFile.Templating.Graph;

namespace FubuCsProjFile.Templating.Runtime
{
    public class ProjectDirectory : IProjectAlteration
    {
        private readonly string _relativePath;

        public ProjectDirectory(string relativePath)
        {
            _relativePath = relativePath.Replace("\\", "/");
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            TemplateLibrary.FileSystem.CreateDirectory(file.ProjectDirectory.AppendPath(_relativePath));
        }

        protected bool Equals(ProjectDirectory other)
        {
            return string.Equals(_relativePath, other._relativePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectDirectory) obj);
        }

        public override int GetHashCode()
        {
            return (_relativePath != null ? _relativePath.GetHashCode() : 0);
        }

        public static IEnumerable<ProjectDirectory> PlanForDirectory(string root)
        {
            return Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
                            .Select(dir => new ProjectDirectory(dir.PathRelativeTo(root)));
        }

        public override string ToString()
        {
            return string.Format("Create folder {0}", _relativePath);
        }
    }
}