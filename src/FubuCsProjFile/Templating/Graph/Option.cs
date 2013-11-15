using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuCsProjFile.Templating.Graph
{
    public class Option : DescribesItself
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

        public Option DescribedAs(string description)
        {
            Description = description;
            return this;
        }

        public void Describe(Description description)
        {
            description.Title = Name;
            description.ShortDescription = Description;
        }
    }
}