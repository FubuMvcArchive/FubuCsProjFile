using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating.Planning
{
    public class FilesTemplatePlanner : ITemplatePlanner
    {
        private readonly Action<TextFile, TemplatePlan> _action;
        private readonly FileSet _matching;

        public FilesTemplatePlanner(FileSet matching, Action<TextFile, TemplatePlan> action)
        {
            _matching = matching;
            _action = action;
        }

        public void DetermineSteps(string directory, TemplatePlan plan)
        {
            TextFile.FileSystem.FindFiles(directory, _matching)
                    .Select(x => new TextFile(x, x.PathRelativeTo(directory)))
                    .Each(file => {
                        _action(file, plan);
                        plan.MarkHandled(file.Path);
                    });
        }
    }
}