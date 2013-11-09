using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;

namespace FubuCsProjFile.Templating.Graph
{
    public static class TemplateGraphExtensions
    {
        public static Option Find(this IList<Option> list, string name)
        {
            return list.FirstOrDefault(x => FubuCore.StringExtensions.EqualsIgnoreCase(x.Name, name));
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

        public static void BuildSelectionsForGenerationType(this XmlElement element, IList<Option> options, GenerationType generation)
        {
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
        }

        public static GenerationType BuildGenerationType(this XmlElement element, IList<Option> options)
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

            element.BuildSelectionsForGenerationType(options, generation);

            return generation;
        }

    }
}