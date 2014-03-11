using System;
using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.Templating.Runtime
{
    public class SystemReference : IProjectAlteration
    {
        private readonly string _assemblyName;
        public const string SourceFile = "references.txt";

        public SystemReference(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public void Alter(IProjectFile file, ProjectPlan plan)
        {
            file.Add<AssemblyReference>(_assemblyName);
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        protected bool Equals(SystemReference other)
        {
            return String.Equals(_assemblyName, other._assemblyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SystemReference) obj);
        }

        public override int GetHashCode()
        {
            return (_assemblyName != null ? _assemblyName.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return String.Format("Add assembly reference to {0}", _assemblyName);
        }
    }
}