using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class SolutionProject
    {
        public static SolutionProject CreateNewAt(string solutionDirectory, string projectName)
        {
            var csProjFile = CsProjFile.CreateAtSolutionDirectory(projectName, solutionDirectory);
            return new SolutionProject(csProjFile, solutionDirectory);
        }


        public static readonly string ProjectLineTemplate = "Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"";
        private readonly Guid _projectType;
        private readonly Guid _projectGuid;
        private readonly string _projectName;
        private readonly string _relativePath;

        private readonly IList<string> _directives = new List<string>();
        private readonly IList<ProjectSection> _projectSections = new List<ProjectSection>(); 
        private readonly Lazy<CsProjFile> _project;

        public SolutionProject(CsProjFile csProjFile, string solutionDirectory)
        {
            _project = new Lazy<CsProjFile>(() => csProjFile);
            _projectName = csProjFile.ProjectName;
            _relativePath = csProjFile.FileName.PathRelativeTo(solutionDirectory);
            _projectType = csProjFile.ProjectTypes().LastOrDefault();
            _projectGuid = csProjFile.ProjectGuid;
        }

        public SolutionProject(string text, string solutionDirectory)
        {
            var parts = text.ToDelimitedArray('=');
            _projectType = Guid.Parse(parts.First().TextBetweenSquiggles());
            _projectGuid = Guid.Parse(parts.Last().TextBetweenSquiggles());

            var secondParts = parts.Last().ToDelimitedArray();
            _projectName = secondParts.First().TextBetweenQuotes();
            _relativePath = secondParts.ElementAt(1).TextBetweenQuotes().Replace("\\", "/"); // Windows is forgiving


            _project = new Lazy<CsProjFile>(() => {
                var filename = solutionDirectory.AppendPath(_relativePath);

                if (File.Exists(filename))
                {
                    var projFile = CsProjFile.LoadFrom(filename);
                    this.InitializeFromSolution(projFile, this.Solution);
                    return projFile;
                }

                var project = CsProjFile.CreateAtLocation(filename, _projectName);
                project.ProjectGuid = _projectGuid;

                return project;
            });
        }

        private void InitializeFromSolution(CsProjFile projFile, Solution solution)
        {
            var tfsSourceControl = solution.Sections.FirstOrDefault(section => section.SectionName.Equals("TeamFoundationVersionControl"));
            if (tfsSourceControl != null)
            {
                this.InitializeTfsSourceControlSettings(projFile, solution, tfsSourceControl);
            }
        }

        private void InitializeTfsSourceControlSettings(CsProjFile projFile, Solution solution, GlobalSection tfsSourceControl)
        {
            var projUnique = tfsSourceControl.Properties.FirstOrDefault(item => item.EndsWith(Path.GetFileName(projFile.FileName)));
            if (projUnique == null)
            {
                return;
            }

            int index =
                Convert.ToInt32(projUnique.Substring("SccProjectUniqueName".Length,
                    projUnique.IndexOf('=') - "SccProjectUniqueName".Length).Trim());

            projFile.SourceControlInformation = new SourceControlInformation(
                tfsSourceControl.Properties.First(item => item.StartsWith("SccProjectUniqueName" + index)).Split('=')[1].Trim(),
                tfsSourceControl.Properties.First(item => item.StartsWith("SccProjectName" + index)).Split('=')[1].Trim(),
                tfsSourceControl.Properties.First(item => item.StartsWith("SccLocalPath" + index)).Split('=')[1].Trim());            
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine(ProjectLineTemplate, _projectType.ToString().ToUpper(), _projectName, _relativePath.Replace('/', Path.DirectorySeparatorChar), _projectGuid.ToString().ToUpper());

            _directives.Each(x => writer.WriteLine(x));
            _projectSections.Each(x => x.Write(writer));

            writer.WriteLine("EndProject");
        }

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

        public CsProjFile Project
        {
            get { return _project.Value; }
        }

        public Solution Solution { get; set; }

        public IList<ProjectSection> ProjectSections
        {
            get { return _projectSections; }
        }

        public ProjectDependenciesSection ProjectDependenciesSection
        {
            get
            {
                return this.ProjectSections.OfType<ProjectDependenciesSection>().FirstOrDefault();
            }
        }

        public void ReadLine(string text)
        {
            _directives.Add(text);
        }

        public void AddProjectDependency(Guid projectGuid)
        {
            if (this.ProjectDependenciesSection == null)
            {
                this.ProjectSections.Add(new ProjectDependenciesSection());
            }

// ReSharper disable once PossibleNullReferenceException, won't be null here, added above
            this.ProjectDependenciesSection.Add(projectGuid);
        }

        public void RemoveProjectDependency(Guid guid)
        {
            if (this.ProjectDependenciesSection == null)
            {
                return;
            }

            this.ProjectDependenciesSection.Remove(guid);
            if (this.ProjectDependenciesSection.Dependencies.Count == 0)
            {
                this.ProjectSections.Remove(this.ProjectDependenciesSection);
            }
        }

        public void RemoveAllProjectDependencies()
        {
            if (this.ProjectDependenciesSection == null)
            {
                return;
            }

            this.ProjectDependenciesSection.Clear();
            this.ProjectSections.Remove(this.ProjectDependenciesSection);
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