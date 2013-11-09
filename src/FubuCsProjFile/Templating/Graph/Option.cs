using System.Collections.Generic;

namespace FubuCsProjFile.Templating.Graph
{
    public class Option
    {
        public string Description;
        public string Name;
        public IList<string> Alterations = new List<string>();
    }
}