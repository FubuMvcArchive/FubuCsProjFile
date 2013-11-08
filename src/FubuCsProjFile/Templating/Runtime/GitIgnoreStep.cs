using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating.Planning;

namespace FubuCsProjFile.Templating.Runtime
{
    public class GitIgnoreStep : ITemplateStep
    {
        public static readonly string File = "ignore.txt";

        private readonly string[] _entries;

        public GitIgnoreStep(params string[] entries)
        {
            _entries = entries;
        }

        public void Alter(TemplatePlan plan)
        {
            plan.AlterFile(".gitignore", list => _entries.Each(list.Fill));
        }

        public string[] Entries
        {
            get { return _entries; }
        }

        public static void ConfigurePlan(TextFile textFile, TemplatePlan plan)
        {
            var ignores = textFile.ReadLines().Where(x => x.IsNotEmpty()).ToArray();
            var step = new GitIgnoreStep(ignores);
            plan.Add(step);
        }

        public override string ToString()
        {
            return string.Format("Adding to .gitignore: {0}", _entries.Join(", "));
        }
    }
}