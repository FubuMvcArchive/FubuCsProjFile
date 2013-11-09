using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;

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

                graph._templateSets.Add(generation);
            }


            return graph;
        }



        private readonly IList<TemplateSet> _templateSets = new List<TemplateSet>();

        public void Add(TemplateSet templateSet)
        {
            _templateSets.Add(templateSet);
        }

        public IEnumerable<TemplateSet> TemplateSetsForTag(string tag)
        {
            return _templateSets.Where(x => x.MatchesTag(tag));
        }

        public TemplateSet TemplateSetFor(string name)
        {
            return _templateSets.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public ProjectRequest Configure(TemplateChoices choices)
        {
            if (choices.SetName.IsEmpty()) throw new Exception("SetName is required");
            if (choices.ProjectName.IsEmpty()) throw new Exception("ProjectName is required");

            var templateSet = TemplateSetFor(choices.SetName);
            if (templateSet == null)
            {
                throw new Exception("TemplateSet '{0}' is unknown".ToFormat(choices.SetName));
            }

            if (choices.Tag.IsNotEmpty() && !templateSet.MatchesTag(choices.Tag))
            {
                throw new Exception("TemplateSet '{0}' is not tagged as a valid '{1}'".ToFormat(choices.SetName, choices.Tag));
            }

            var request = new ProjectRequest(choices.ProjectName, templateSet.Template);
            request.Alterations.AddRange(templateSet.Alterations);

            if (choices.Options != null)
            {
                choices.Options.Each(o => {
                    var opt = templateSet.FindOption(o);
                    if (opt == null) throw new Exception("Unknown option '{0}'".ToFormat(o));

                    request.Alterations.AddRange(opt.Alterations);
                });
            }

            if (templateSet.Selections != null)
            {
                templateSet.Selections.Each(selection => selection.Configure(choices, request));
            }

            choices.Inputs.Each((key, value) => request.Substitutions.Set(key, value));

            return request;
        }
    }
}