using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;

namespace FubuCsProjFile.Templating.Graph
{
    public class TemplateGraph
    {
        public static TemplateGraph Read(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            var graph = new TemplateGraph();

            var options = document.ReadOptions();

            foreach (XmlElement element in document.DocumentElement.SelectNodes("generation"))
            {
                var generation = element.BuildGenerationType(options);

                graph._generationTypes.Add(generation);
            }


            return graph;
        }



        private readonly IList<GenerationType> _generationTypes = new List<GenerationType>(); 

        public IEnumerable<GenerationType> GenerationTypesForTag(string tag)
        {
            return _generationTypes.Where(x => x.Tags.Any(t => t.EqualsIgnoreCase(tag)));
        }

        public GenerationType GenerationTypeFor(string name)
        {
            return _generationTypes.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }
    }

    public class Option
    {
        public string Description;
        public string Name;
        public IList<string> Alterations = new List<string>();
    }

    public class OptionSelection
    {
        public string Description;
        public string Name;
        public IList<Option> Options = new List<Option>();
    }

    public class GenerationType
    {
        // If this is null, it's not a "new" project
        public string Template;
        public IList<string> Tags = new List<string>();

        public IList<string> Alterations = new List<string>();
        public string Description;
        public string Name;

        public IList<Option> Options = new List<Option>();
        public IList<OptionSelection> Selections = new List<OptionSelection>();
    }

    public class TemplateChoices
    {
        //public string 
    }
}