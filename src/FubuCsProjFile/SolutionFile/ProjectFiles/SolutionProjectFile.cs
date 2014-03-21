using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFile : SolutionProject, ISolutionProjectFile
    {
        private readonly Guid _projectTypeFromSolutionFile;
        private readonly Lazy<IProjectFile> _projectFile;

        public SolutionProjectFile(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution, IFileSystem fileSystem) : base(projectGuid, projectName, relativePath)
        {
            _projectTypeFromSolutionFile = projectType;

            _projectFile = new Lazy<IProjectFile>(() =>
            {
                var filename = solution.Filename.ParentDirectory().AppendPath(relativePath);

                if (fileSystem.FileExists(filename))
                {
                    var projFile = ProjectLoader.Load(filename);
                    InitializeFromSolution(projFile, solution);
                    return projFile;
                }

                var project = ProjectCreator.CreateAtLocation(filename, projectName);
                project.As<IInternalProjectFile>().SetProjectGuid(projectGuid);

                return project;
            });
        }

        public SolutionProjectFile(IProjectFile project, string solutionDirectory) : base(project.ProjectGuid, project.ProjectName, project.FileName.PathRelativeTo(solutionDirectory))
        {
            _projectFile = new Lazy<IProjectFile>(() => project);
            var seed = _projectFile.Value;
        }

        public override Guid Type
        {
            get
            {
                return _projectFile.IsValueCreated
                    ? _projectFile.Value.Type
                    : _projectTypeFromSolutionFile;
            }
        }

        public IProjectFile Project { get { return _projectFile.Value; } }

        public void Save()
        {
            if (!_projectFile.IsValueCreated)
            {
                return;
            }

            _projectFile.Value.Save();
        }

        private void InitializeFromSolution(IProjectFile projFile, ISolution solution)
        {
            var tfsSourceControl = solution.Sections.FirstOrDefault(section => section.SectionName.Equals("TeamFoundationVersionControl"));
            if (tfsSourceControl != null)
            {
                InitializeTfsSourceControlSettings(projFile, solution, tfsSourceControl);
            }
        }

        private void InitializeTfsSourceControlSettings(IProjectFile projFile, ISolution solution, GlobalSection tfsSourceControl)
        {
            var projUnique = tfsSourceControl.Properties.FirstOrDefault(item => item.EndsWith(Path.GetFileName(projFile.FileName)));
            if (projUnique == null)
            {
                return;
            }

            var index = tfsSourceControl.Properties.IndexOf(projUnique);
            projFile.SourceControlInformation = new SourceControlInformation(
                tfsSourceControl.Properties[index].Split('=')[1].Trim(),
                tfsSourceControl.Properties[index + 1].Split('=')[1].Trim(),
                tfsSourceControl.Properties[index + 2].Split('=')[1].Trim());
        }
    }
}