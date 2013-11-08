using System;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating.Graph
{
    public class ProjectRequest
    {
        private readonly IList<string> _alterations = new List<string>(); 
        private readonly Substitutions _substitutions = new Substitutions();

        public ProjectRequest(string name, string template)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (template == null) throw new ArgumentNullException("template");

            Name = name;
            Template = template;
        }

        public string Name { get; private set; }
        public string Template { get; private set; }

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