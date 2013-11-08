using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;

namespace FubuCsProjFile.Templating.Graph
{
    public static class TemplateGraphExtensions
    {
        public static Option Find(this IList<Option> list, string name)
        {
            return list.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public static IList<Option> ReadOptions(this XmlDocument document)
        {
            var options = new List<Option>();
            foreach (XmlElement element in document.DocumentElement.SelectNodes("option"))
            {
                var option = new Option
                {
                    Name = element.GetAttribute("name"),
                    Description = element.GetAttribute("description"),
                    Alterations = element.GetAttribute("alterations").ToDelimitedArray().ToList()
                };

                options.Add(option);
            }

            return options;
        } 
    }

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
                var generation = new GenerationType();
                generation.Name = element.GetAttribute("name");
                generation.Description = element.GetAttribute("description");
                generation.Template = element.GetAttribute("template");

                if (element.HasAttribute("alterations"))
                {
                    generation.Alterations.AddRange(element.GetAttribute("alterations").ToDelimitedArray());
                }

                if (element.HasAttribute("tags"))
                {
                    generation.Tags.AddRange(element.GetAttribute("tags").ToDelimitedArray());
                }

                if (element.HasAttribute("options"))
                {
                    element.GetAttribute("options").ToDelimitedArray()
                        .Select(x => options.Find(x))
                        .Each(x => generation.Options.Add(x));
                }

                foreach (XmlElement selectionElement in element.SelectNodes("selection"))
                {
                    var selection = new OptionSelection
                    {
                        Name = selectionElement.GetAttribute("name"),
                        Description = selectionElement.GetAttribute("description")
                    };

                    selectionElement.GetAttribute("options").ToDelimitedArray()
                        .Select(x => options.Find(x))
                        .Each(x => selection.Options.Add(x));

                    generation.Selections.Add(selection);
                }

                graph._generationTypes.Add(generation);
            }




            return graph;
        }

        private readonly IList<GenerationType> _generationTypes = new List<GenerationType>(); 

        public IEnumerable<GenerationType> GenerationTypesForTag(string tag)
        {
            throw new NotImplementedException();
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
    }
}