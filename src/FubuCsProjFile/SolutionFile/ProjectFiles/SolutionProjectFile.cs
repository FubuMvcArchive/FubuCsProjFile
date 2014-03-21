using System;
using FubuCore;
using FubuCsProjFile.ProjectFiles;

namespace FubuCsProjFile.SolutionFile.ProjectFiles
{
    public class SolutionProjectFile : SolutionProject, ISolutionProjectFile
    {
        private readonly Guid _projectTypeFromSolutionFile;
        private readonly Lazy<IProjectFile> _projectFile;

        public SolutionProjectFile(
            Guid projectType,
            Guid projectGuid,
            string projectName,
            string relativePath,
            ISolution solution)
            : base(projectGuid, projectName, relativePath)
        {
            _projectTypeFromSolutionFile = projectType;

            _projectFile = new Lazy<IProjectFile>(() =>
                ProjectLoader.LoadOrCreateIfNotFound(projectGuid, projectName, relativePath, solution));
        }

        public SolutionProjectFile(IProjectFile project, string solutionDirectory)
            : base(project.ProjectGuid, project.ProjectName, project.FileName.PathRelativeTo(solutionDirectory))
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
    }
}