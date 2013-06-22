using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class ProjectRequest
    {
        private readonly IList<string> _alterations = new List<string>(); 

        public string Name { get; set; }
        public string Template { get; set; }

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