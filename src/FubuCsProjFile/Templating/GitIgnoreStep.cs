using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class GitIgnoreStep : ITemplateStep
    {
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
    }
}