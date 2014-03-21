using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;
using FubuCsProjFile.ProjectFiles;
using FubuCsProjFile.SolutionFile;

namespace FubuCsProjFile
{
    [MarkedForTermination]
    public class SolutionProject : ISolutionProjectFile
    {
        public static ISolutionProjectFile CreateNewAt(string solutionDirectory, string projectName, ProjectType type)
        {
            throw new NotImplementedException();
//            var csProjFile = ProjectCreator.CreateAtSolutionDirectory(projectName, solutionDirectory, type);
//            return new SolutionProject(csProjFile, solutionDirectory);
        }

        public static readonly string ProjectLineTemplate = "Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"";
        private readonly Guid _projectType;
        private readonly Guid _projectGuid;
        private readonly string _projectName;
        private readonly string _relativePath;

        private readonly IList<string> _directives = new List<string>();
        private readonly Lazy<IProjectFile> _project;

        public SolutionProject(IProjectFile projectFile, string solutionDirectory)
        {
            _project = new Lazy<IProjectFile>(() => projectFile);
            _projectName = projectFile.ProjectName;
            _relativePath = projectFile.FileName.PathRelativeTo(solutionDirectory);
            _projectType = projectFile.ProjectTypes().LastOrDefault();
            _projectGuid = projectFile.ProjectGuid;
        }

        public SolutionProject(string text, string solutionDirectory) : this(text, solutionDirectory, ProjectFiles.ProjectType.CsProj)
        {

        }

        public SolutionProject(string text, string solutionDirectory, ProjectType type) : this(text, solutionDirectory, type, new FileSystem())
        {

        }

        public SolutionProject(string text, string solutionDirectory, ProjectType type, IFileSystem fileSystem)
        {
            var parts = text.ToDelimitedArray('=');
            _projectType = Guid.Parse(parts.First().TextBetweenSquiggles());
            _projectGuid = Guid.Parse(parts.Last().TextBetweenSquiggles());

            var secondParts = parts.Last().ToDelimitedArray();
            _projectName = secondParts.First().TextBetweenQuotes();
            _relativePath = secondParts.ElementAt(1).TextBetweenQuotes().Replace("\\", "/"); // Windows is forgiving

            _project = new Lazy<IProjectFile>(() => {
                var filename = solutionDirectory.AppendPath(_relativePath);

                if (fileSystem.FileExists(filename))
                {
                    var projFile = ProjectLoader.Load(filename);
                    InitializeFromSolution(projFile, this.Solution);
                    return projFile;
                }

                var project = ProjectCreator.CreateAtLocation(filename, _projectName, type, _projectGuid);

                return project;
            });
        }

        private void InitializeFromSolution(IProjectFile projFile, Solution solution)
        {
            var tfsSourceControl = solution.Sections.FirstOrDefault(section => section.SectionName.Equals("TeamFoundationVersionControl"));
            if (tfsSourceControl != null)
            {
                this.InitializeTfsSourceControlSettings(projFile, solution, tfsSourceControl);
            }
        }

        private void InitializeTfsSourceControlSettings(IProjectFile projFile, Solution solution, GlobalSection tfsSourceControl)
        {
            var projUnique = tfsSourceControl.Properties.FirstOrDefault(item => item.EndsWith(Path.GetFileName(projFile.FileName)));
            if (projUnique == null)
            {
                return;
            }

            int index = tfsSourceControl.Properties.IndexOf(projUnique);
            projFile.SourceControlInformation = new SourceControlInformation(
                tfsSourceControl.Properties[index].Split('=')[1].Trim(),
                tfsSourceControl.Properties[index + 1].Split('=')[1].Trim(),
                tfsSourceControl.Properties[index + 2].Split('=')[1].Trim());            
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine(ProjectLineTemplate, _projectType.ToString().ToUpper(), _projectName, _relativePath.Replace('/', Path.DirectorySeparatorChar), _projectGuid.ToString().ToUpper());

            _directives.Each(x => writer.WriteLine(x));

            writer.WriteLine("EndProject");
        }

        public Guid Type { get; private set; }

        public Guid ProjectGuid
        {
            get { return _projectGuid; }
        }

        public Guid ProjectType
        {
            get { return _projectType; }
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void ForSolutionFile(StringWriter writer)
        {
            Write(writer);
        }

        public IProjectFile Project
        {
            get { return _project.Value; }
        }

        public void Save()
        {
            if (!_project.IsValueCreated)
            {
                return;
            }

            _project.Value.Save();
        }

        public Solution Solution { get; set; }

        public void ReadLine(string text)
        {
            _directives.Add(text);
        }


    }

    public static class CsProjFileExtensions
    {
        public static string TextBetweenSquiggles(this string text)
        {
            var start = text.IndexOf("{");
            var end = text.IndexOf("}");

            return text.Substring(start + 1, end - start - 1);
        }

        public static string TextBetweenQuotes(this string text)
        {
            return text.Trim().TrimStart('"').TrimEnd('"');
        }
    }
}
