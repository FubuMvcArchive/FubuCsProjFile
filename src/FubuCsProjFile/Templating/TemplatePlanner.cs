using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public abstract class TemplatePlanner : ITemplatePlannerAction
    {
        private readonly IList<ITemplatePlanner> _planners = new List<ITemplatePlanner>();
        private FileSet _matching;

        public void CreatePlan(string directory, TemplatePlan plan)
        {
            _planners.Each(x => x.DetermineSteps(directory, plan));
        }

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