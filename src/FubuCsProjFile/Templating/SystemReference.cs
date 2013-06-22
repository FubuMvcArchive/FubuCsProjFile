namespace FubuCsProjFile.Templating
{
    public class SystemReference : IProjectAlteration
    {
        private readonly string _assemblyName;

        public SystemReference(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            file.Add<AssemblyReference>(_assemblyName);
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        protected bool Equals(SystemReference other)
        {
            return string.Equals(_assemblyName, other._assemblyName);
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
            return string.Format("System Assembly Reference to {0}", _assemblyName);
        }
    }
}