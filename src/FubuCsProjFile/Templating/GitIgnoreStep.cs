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

        public void Alter(TemplateContext context)
        {
            context.AlterFile(".gitignore", list => _entries.Each(list.Fill));
        }

        public string[] Entries
        {
            get { return _entries; }
        }

        public static void ConfigurePlan(string directory, TemplateContext context)
        {
            var filename = directory.AppendPath(File);
            context.MarkHandled(filename);

            new FileSystem().AlterFlatFile(filename, list => {
                if (list.Any())
                {
                    var step = new GitIgnoreStep(list.Where(x => x.IsNotEmpty()).ToArray());
                    context.Add(step);
                }
            });
        }
    }
}