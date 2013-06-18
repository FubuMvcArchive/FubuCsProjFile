using System.Collections.Generic;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile.Templating
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

        public static void ConfigurePlan(string directory, TemplatePlan plan)
        {
            var filename = directory.AppendPath(File);
            plan.MarkHandled(filename);

            new FileSystem().AlterFlatFile(filename, list => {
                if (list.Any())
                {
                    var step = new GitIgnoreStep(list.Where(x => x.IsNotEmpty()).ToArray());
                    plan.Add(step);
                }
            });
        }
    }
}