using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class ProjectRequest
    {
        private readonly IList<string> _alterations = new List<string>(); 
        private readonly Substitutions _substitutions = new Substitutions();

        public string Name { get; set; }
        public string Template { get; set; }

        public Substitutions Substitutions
        {
            get { return _substitutions; }
        }

        public IEnumerable<string> Alterations
        {
            get { return _alterations; }
            set
            {
                _alterations.Clear();
                _alterations.AddRange(value);
            }
        } 

        public void AddAlteration(string alteration)
        {
            _alterations.Add(alteration);
        }
    }
}