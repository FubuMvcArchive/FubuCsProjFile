using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public static IList<Option> ReadOptions(this XmlElement parentElement)
        {
            var options = new List<Option>();
            foreach (XmlElement element in parentElement.SelectNodes("option"))
            {
                var option = new Option
                {
                    Name = element.GetAttribute("name"),
                    Description = element.GetAttribute("description"),
                    Alterations = element.GetAttribute("alterations").ToDelimitedArray().ToList(),
                    Url = element.GetAttribute("url")
                    
                };

                options.Add(option);
            }

            return options;
        } 

        public static IEnumerable<OptionSelection> BuildSelections(this XmlElement element)
        {
            foreach (XmlElement selectionElement in element.SelectNodes("selection"))
            {
                yield return selectionElement.BuildSelection();
            }
        }

        public static OptionSelection BuildSelection(this XmlElement selectionElement)
        {
            var selection = new OptionSelection
            {
                Name = selectionElement.GetAttribute("name"),
                Description = selectionElement.GetAttribute("description")
            };

            selection.Options = selectionElement.ReadOptions();

            return selection;
        }

        public static ProjectTemplate BuildProjectTemplate(this XmlElement element)
        {
            var projectTemplate = new ProjectTemplate
            {
                Name = element.GetAttribute("name"),
                Description = element.GetAttribute("description"),
                Template = element.GetAttribute("template"),
                DotNetVersion = element.GetAttribute("dotnet")
            };

            if (element.HasAttribute("alterations"))
            {
                projectTemplate.Alterations.AddRange(element.GetAttribute("alterations").ToDelimitedArray());
            }

            projectTemplate.Url = element.GetAttribute("url");
            projectTemplate.Options = element.ReadOptions();

            projectTemplate.Selections.AddRange(element.BuildSelections());

            return projectTemplate;
        }

    }
}