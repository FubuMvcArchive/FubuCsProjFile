using System.Collections.Generic;

namespace FubuCsProjFile.Templating.Graph
{
    public class Option
    {
        public Option()
        {
        }

        public Option(string name, params string[] alterations)
        {
            Name = name;
            Alterations.AddRange(alterations);
        }

        public string Description;
        public string Name;
        public IList<string> Alterations = new List<string>();
    }
}