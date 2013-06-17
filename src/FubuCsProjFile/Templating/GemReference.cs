using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class GemReference : ITemplateStep
    {
        public static readonly string DefaultFeed = "source 'http://rubygems.org'";

        public GemReference()
        {
        }

        public GemReference(string gemName, string version)
        {
            GemName = gemName;
            Version = version;
        }

        public string GemName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplateContext context)
        {
            context.AlterFile("Gemfile", Alter);
        }

        private void Alter(List<string> list)
        {
            if (!list.Contains(DefaultFeed))
            {
                list.Insert(0, DefaultFeed);
                list.Insert(1, string.Empty);
            }

            var key = "\"{0}\"".ToFormat(GemName);
            if (list.Any(x => x.Contains(key)))
            {
                // TODO -- trace
            }
            else
            {
                var line = "gem {0}, \"{1}\"".ToFormat(key, Version);
                list.Add(line);
            }
        }
    }
}