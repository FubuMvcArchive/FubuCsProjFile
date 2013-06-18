using System;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class TemplateBuilder
    {
        private readonly IList<ITemplatePlanner> _all = new List<ITemplatePlanner>();

        protected TemplatePlannerExpression All
        {
            get
            {
                return new TemplatePlannerExpression(_all);
            }
        }



        /*
         * .cs files are CodeFileTemplate
         * description.txt is ignored
         * ignore.txt gets written to .gitignore in the solution
         * nuget.txt gets added to ripple and/or nuget dependencies
         * gems.txt gets added to Gemfile
         * files we don't recognize just get copied
         * references.txt <-- adds system assembly references
         * csproj.xml -- csproj
    
         * ****AssemblyInfo.cs is combined
         */

        public void ConfigureTree(string directory, TemplatePlan plan)
        {
            GemReference.ConfigurePlan(directory, plan);
            GitIgnoreStep.ConfigurePlan(directory, plan);

            plan.CopyUnhandledFilesToRoot(directory);
        }

        public void ConfigureProject(string directory, ProjectPlan projectPlan, TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITemplatePlannerAction
    {
        Action<TextFile, TemplatePlan> Do { set; }
    }

    public class TemplatePlannerExpression : ITemplatePlannerAction
    {
        private readonly IList<ITemplatePlanner> _planners;
        private FileSet _matching;

        public TemplatePlannerExpression(IList<ITemplatePlanner> planners)
        {
            _planners = planners;
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



    public interface ITemplatePlanner
    {
        void DetermineSteps(string directory, TemplatePlan plan);
    }

    public class FilesTemplatePlanner : ITemplatePlanner
    {
        private readonly Action<TextFile, TemplatePlan> _action;
        private FileSet _matching;

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