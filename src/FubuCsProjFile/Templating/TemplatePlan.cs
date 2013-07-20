using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TemplatePlan
    {
        public const string SOLUTION_NAME = "%SOLUTION_NAME%";
        public const string SOLUTION_PATH = "%SOLUTION_PATH%";
        public const string INSTRUCTIONS = "%INSTRUCTIONS%";


        public static readonly string RippleImportFile = "ripple-install.txt";
        public static readonly string InstructionsFile = "instructions.txt";

        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly IList<string> _handled = new List<string>();
        private readonly StringWriter _instructions = new StringWriter();
        private readonly IList<string> _missingInputs = new List<string>();
        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>();
        private readonly Substitutions _substitutions = new Substitutions();
        private ProjectPlan _currentProject;
        private Solution _solution;


        public TemplatePlan(string rootDirectory)
        {
            Root = rootDirectory;
            SourceName = "src";

            Logger = new TemplateLogger();
        }

        public IList<string> MissingInputs
        {
            get { return _missingInputs; }
        }

        public Substitutions Substitutions
        {
            get { return _substitutions; }
        }

        public ITemplateLogger Logger { get; private set; }

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

        public IEnumerable<ITemplateStep> Steps
        {
            get { return _steps; }
        }

        public ProjectPlan CurrentProject
        {
            get
            {
                
                // Hokey, but leave it
                return _currentProject ?? _steps.OfType<ProjectPlan>().LastOrDefault();
            }
        }

        public static TemplatePlan CreateClean(string directory)
        {
            var system = new FileSystem();
            system.CreateDirectory(directory);
            system.CleanDirectory(directory);

            return new TemplatePlan(directory);
        }

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

        public void Add(ITemplateStep step)
        {
            _steps.Add(step);
        }

        public void StartProject(ProjectPlan project)
        {
            _currentProject = project;
        }

        public void Execute()
        {
            if (_missingInputs.Any())
            {
                Logger.Trace("Missing Inputs:");
                Logger.Trace("---------------");
                _missingInputs.Each(x => Console.WriteLine(x));

                throw new MissingInputException(_missingInputs.ToArray());
            }

            Logger.Starting(_steps.Count);
            _substitutions.Trace(Logger);

            _substitutions.Set(INSTRUCTIONS, GetInstructions().Replace("\"", "'"));

            _steps.Each(x => {
                Logger.TraceStep(x);
                x.Alter(this);
            });

            if (Solution != null)
            {
                Logger.Trace("Saving solution to {0}", Solution.Filename);
                Solution.Save();
            }

            Substitutions.WriteTo(Root.AppendPath(Substitutions.ConfigFile));
            WriteNugetImports();

            Logger.Finish();
        }

        public void WritePreview()
        {
            Logger.Starting(_steps.Count);
            _substitutions.Trace(Logger);

            _steps.Each(x => {
                Logger.TraceStep(x);
                var project = x as ProjectPlan;


                if (project != null)
                {
                    Logger.StartProject(project.Alterations.Count);
                    project.Substitutions.Trace(Logger);
                    project.Alterations.Each(alteration => Logger.TraceAlteration(ApplySubstitutions(alteration.ToString())));
                    Logger.EndProject();
                }
            });

            string[] projectsWithNugets = determineProjectsWithNugets();
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
            if (Path.GetFileName(file).ToLowerInvariant() == InstructionsFile) return false;

            string path = file.CanonicalPath();
            return !_handled.Contains(path);
        }

        public void CopyUnhandledFiles(string directory)
        {
            IEnumerable<string> unhandledFiles =
                _fileSystem.FindFiles(directory, FileSet.Everything()).Where(FileIsUnhandled);

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
            string[] projectsWithNugets = determineProjectsWithNugets();

            if (projectsWithNugets.Any())
            {
                Logger.Trace("Writing nuget imports:");
                projectsWithNugets.Each(x => Logger.Trace(x));
                Logger.Trace("");

                TemplateLibrary.FileSystem.AlterFlatFile(Root.AppendPath(RippleImportFile),
                                                         list => { list.AddRange(projectsWithNugets); });
            }
        }

        private string[] determineProjectsWithNugets()
        {
            string[] projectsWithNugets = Steps
                .OfType<ProjectPlan>()
                .Where(x => x.NugetDeclarations.Any())
                .Select(x => x.ToNugetImportStatement()).ToArray();
            return projectsWithNugets;
        }

        public ProjectPlan FindProjectPlan(string name)
        {
            return _steps.OfType<ProjectPlan>().FirstOrDefault(x => x.ProjectName == name);
        }

        public void AddInstructions(string rawText)
        {
            _instructions.WriteLine(rawText);
            _instructions.WriteLine();
            _instructions.WriteLine();
        }


        public void WriteInstructions()
        {
            if (_instructions.ToString().IsEmpty()) return;

            var instructionText = GetInstructions();
            var contents = instructionText.SplitOnNewLine();

            FileSystem.AlterFlatFile(Root.AppendPath(InstructionsFile),
                                     list => list.AddRange(contents));


            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            contents.Each(x => Console.WriteLine(x));

            Console.ResetColor();
        }

        public string GetInstructions()
        {
            return ApplySubstitutions(_instructions.ToString());
        }
    }
}