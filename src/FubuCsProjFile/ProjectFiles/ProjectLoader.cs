using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.ProjectFiles.CsProj;
using FubuCsProjFile.SolutionFile;

namespace FubuCsProjFile.ProjectFiles
{
    public static class ProjectLoader
    {
        private static IFileSystem fileSystem = new FileSystem();

        private static readonly IList<IProjectLoader> Loaders = new List<IProjectLoader>
        {
            new CsProjLoader(fileSystem)
        }; 

        public static IProjectFile Load(string filename)
        {
            if (!fileSystem.FileExists(filename)) return null;

            var loader = Loaders.FirstOrDefault(x => x.Matches(filename));

            if (loader == null) return null;
                
            return loader.Load(filename);
        }

        public static IProjectFile LoadOrCreateIfNotFound(Guid projectGuid, string projectName, string relativePath, ISolution solution)
        {
            var filename = solution.ParentDirectory.AppendPath(relativePath);

            if (fileSystem.FileExists(filename))
            {
                var project = Load(filename);
                InitializeFromSolution(project, solution);
                return project;
            }

            return ProjectCreator.CreateAtLocation(filename, projectName, projectGuid);
        }

        private static void InitializeFromSolution(IProjectFile projFile, ISolution solution)
        {
            var tfsSourceControl = solution.Sections.FirstOrDefault(section => section.SectionName.Equals("TeamFoundationVersionControl"));
            if (tfsSourceControl != null)
            {
                InitializeTfsSourceControlSettings(projFile, tfsSourceControl);
            }
        }

        private static void InitializeTfsSourceControlSettings(IProjectFile projFile, GlobalSection tfsSourceControl)
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