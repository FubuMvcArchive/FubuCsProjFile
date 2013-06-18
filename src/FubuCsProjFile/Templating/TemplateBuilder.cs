using System;
using FubuCore;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating
{
    public class TemplateBuilder
    {


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
            new GenericPlanner().CreatePlan(directory, plan);

            // Need to do something similar for projects
            plan.CopyUnhandledFilesToRoot(directory);
        }

        public void ConfigureProject(string directory, ProjectPlan projectPlan, TemplatePlan plan)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericPlanner : TemplatePlanner
    {
        public GenericPlanner()
        {
            ShallowMatch(GemReference.File).Do = GemReference.ConfigurePlan;
            ShallowMatch(GitIgnoreStep.File).Do = GitIgnoreStep.ConfigurePlan;
        }
    }

    public interface ITemplatePlannerAction
    {
        Action<TextFile, TemplatePlan> Do { set; }
    }

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

    

    public interface ITemplatePlanner
    {
        void DetermineSteps(string directory, TemplatePlan plan);
    }

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