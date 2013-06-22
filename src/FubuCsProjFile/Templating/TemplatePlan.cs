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
        }

        public string ApplySubstitutions(string rawText)
        {
            var builder = new StringBuilder(rawText);

            if (CurrentProject != null)
            {
                CurrentProject.ApplySubstitutions(null, builder);
            }

            if (Solution != null)
            {
                builder.Replace(SOLUTION_NAME, Solution.Name);
                builder.Replace(SOLUTION_PATH, Solution.Filename.PathRelativeTo(Root).Replace("\\", "/"));
            }

            return builder.ToString();
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

        public Solution Solution { get; set; }

        public IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>();
        private ProjectPlan _currentProject;


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
            _steps.Each(x => x.Alter(this));

            if (Solution != null)
            {
                Solution.Save();
            }
        }

        public void AlterFile(string relativeName, Action<List<string>> alter)
        {
            _fileSystem.AlterFlatFile(Root.AppendPath(relativeName), alter);
        }

        public bool FileIsUnhandled(string file)
        {
            if (Path.GetFileName(file).ToLowerInvariant() == TemplateLibrary.DescriptionFile) return false;

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
            var projectsWithNugets = Steps
                .OfType<ProjectPlan>()
                .Where(x => x.NugetDeclarations.Any())
                .Select(x => x.ToNugetImportStatement()).ToArray();

            if (projectsWithNugets.Any())
            {
                TemplateLibrary.FileSystem.AlterFlatFile(Root.AppendPath(RippleImportFile), list => {
                    list.AddRange(projectsWithNugets);
                });
            }
        }
    }
}