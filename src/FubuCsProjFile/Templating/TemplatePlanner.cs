using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public abstract class TemplatePlanner : ITemplatePlannerAction
    {
        private readonly IList<ITemplatePlanner> _planners = new List<ITemplatePlanner>();
        private FileSet _matching;

        protected TemplatePlanner()
        {
            ShallowMatch(GemReference.File).Do = GemReference.ConfigurePlan;
            ShallowMatch(GitIgnoreStep.File).Do = GitIgnoreStep.ConfigurePlan;

            ShallowMatch(RakeFileTransform.SourceFile).Do = (file, plan) => {
                plan.Add(new RakeFileTransform(file.ReadAll()));
            };


        }

        public void CreatePlan(Template template, TemplatePlan plan)
        {
            configurePlan(template.Path, plan);

            _planners.Each(x => x.DetermineSteps(template.Path, plan));

            plan.CopyUnhandledFiles(template.Path);
        }

        protected abstract void configurePlan(string directory, TemplatePlan plan);

        public void Add<T>() where T : ITemplatePlanner, new()
        {
            _planners.Add(new T());
        }

        public ITemplatePlannerAction Matching(FileSet matching)
        {
            _matching = matching;
            return this;
        }

        public ITemplatePlannerAction DeepMatch(string pattern)
        {
            return Matching(FileSet.Deep(pattern));
        }

        public ITemplatePlannerAction ShallowMatch(string pattern)
        {
            return Matching(FileSet.Shallow(pattern));
        }

        public Action<TextFile, TemplatePlan> Do
        {
            set 
            { 
                var planner = new FilesTemplatePlanner(_matching, value);
                _planners.Add(planner);
            }
        }
    }
}