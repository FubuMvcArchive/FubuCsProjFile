using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile.Templating
{
    public class TemplatePlan
    {
        public const string SOLUTION_NAME = "%SOLUTION_NAME%";
        public const string SOLUTION_PATH = "%SOLUTION_PATH%";


        public static readonly string RippleImportFile = "ripple-install.txt";

        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly IList<string> _handled = new List<string>(); 
        private readonly Substitutions _substitutions = new Substitutions();

        public static TemplatePlan CreateClean(string directory)
        {
            var system = new FileSystem();
            system.CreateDirectory(directory);
            system.CleanDirectory(directory);

            return new TemplatePlan(directory);
        }

        public TemplatePlan(string rootDirectory)
        {
            Root = rootDirectory;
            SourceName = "src";

            Logger = new TemplateLogger();
        }

        public Substitutions Substitutions
        {
            get { return _substitutions; }
        }

        public ITemplateLogger Logger { get; private set; }

        public string ApplySubstitutions(string rawText)
        {
            return _substitutions.ApplySubstitutions(rawText, builder => {
                if (CurrentProject != null)
                {
                    CurrentProject.ApplySubstitutions(null, builder);
                }
            });
        }



        public void MarkHandled(string file)
        {
            _handled.Add(file.CanonicalPath());
        }

        public string Root { get; set; }

        // TODO -- this will have to be settable from the TemplateRequest!  Or read some how.
        public string SourceName { get; set; }

        public string SourceDirectory
        {
            get { return Root.AppendPath(SourceName); }
        }

        public Solution Solution
        {
            get { return _solution; }
            set
            {
                _solution = value;
                _substitutions.Set(SOLUTION_NAME, _solution.Name);
                _substitutions.Set(SOLUTION_PATH, Solution.Filename.PathRelativeTo(Root).Replace("\\", "/"));
            }
        }

        public IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>();
        private ProjectPlan _currentProject;
        private Solution _solution;


        public void Add(ITemplateStep step)
        {
            if (step is ProjectPlan)
            {
                _currentProject = step.As<ProjectPlan>();
            }

            _steps.Add(step);
        }

        public IEnumerable<ITemplateStep> Steps
        {
            get { return _steps; }
        }

        public ProjectPlan CurrentProject
        {
            get { return _currentProject; }
        }

        public void Execute()
        {
            Logger.Starting(_steps.Count);
            _steps.Each(x => {
                Logger.TraceStep(x);
                x.Alter(this);
            });

            if (Solution != null)
            {
                Logger.Trace("Saving solution to {0}", Solution.Filename);
                Solution.Save();
            }

            WriteNugetImports();

            Logger.Finish();
        }

        public void WritePreview()
        {
            Logger.Starting(_steps.Count);

            _steps.Each(x => {
                Logger.TraceStep(x);
                var project = x as ProjectPlan;
                if (project != null)
                {
                    Logger.StartProject(project.Alterations.Count);
                    project.Alterations.Each(alteration => Logger.TraceAlteration(alteration));
                    Logger.EndProject();
                }
            });

            var projectsWithNugets = determineProjectsWithNugets();
            if (projectsWithNugets.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Nuget imports:");
                projectsWithNugets.Each(x => Console.WriteLine(x));
            }

        }

        public void AlterFile(string relativeName, Action<List<string>> alter)
        {
            _fileSystem.AlterFlatFile(Root.AppendPath(relativeName), alter);
        }

        public bool FileIsUnhandled(string file)
        {
            if (Path.GetFileName(file).ToLowerInvariant() == TemplateLibrary.DescriptionFile) return false;
            if (Path.GetFileName(file).ToLowerInvariant() == Input.File) return false;

            var path = file.CanonicalPath();
            return !_handled.Contains(path);
        }

        public void CopyUnhandledFiles(string directory)
        {
            var unhandledFiles = _fileSystem.FindFiles(directory, FileSet.Everything()).Where(FileIsUnhandled);

            if (CurrentProject == null)
            {
                unhandledFiles.Each(file => Add(new CopyFileToSolution(file.PathRelativeTo(directory), file)));
            }
            else
            {
                unhandledFiles.Each(
                    file => CurrentProject.Add(new CopyFileToProject(file.PathRelativeTo(directory), file)));
            }


        }

        public void WriteNugetImports()
        {
            var projectsWithNugets = determineProjectsWithNugets();

            if (projectsWithNugets.Any())
            {
                Logger.Trace("Writing nuget imports:");
                projectsWithNugets.Each(x => Logger.Trace(x));
                Logger.Trace("");

                TemplateLibrary.FileSystem.AlterFlatFile(Root.AppendPath(RippleImportFile), list => {
                    list.AddRange(projectsWithNugets);
                });
            }
        }

        private string[] determineProjectsWithNugets()
        {
            var projectsWithNugets = Steps
                .OfType<ProjectPlan>()
                .Where(x => x.NugetDeclarations.Any())
                .Select(x => x.ToNugetImportStatement()).ToArray();
            return projectsWithNugets;
        }

        public ProjectPlan FindProjectPlan(string name)
        {
            return _steps.OfType<ProjectPlan>().FirstOrDefault(x => x.ProjectName == name);
        }
    }
}