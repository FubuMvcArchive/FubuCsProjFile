using System;
using System.IO;

namespace FubuCsProjFile.SolutionFile
{
    public abstract class SolutionProject : ISolutionProject
    {
        public const string ProjectLineTemplate = "Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"";

        public abstract Guid Type { get; }
        public Guid ProjectGuid { get; private set; }
        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }

        protected SolutionProject(Guid projectGuid, string projectName, string relativePath)
        {
            ProjectGuid = projectGuid;
            ProjectName = projectName;
            RelativePath = relativePath;
        }

        public void ForSolutionFile(StringWriter writer)
        {
            writer.WriteLine(ProjectLineTemplate,
                Type.ToString().ToUpper(),
                ProjectName,
                RelativePath.Replace('/', Path.DirectorySeparatorChar),
                ProjectGuid.ToString().ToUpper());

            ProjectDataForSolutionFile(writer);

            writer.WriteLine("EndProject");
        }

        protected virtual void ProjectDataForSolutionFile(StringWriter writer)
        {
            // NO-OP
        }
    }
}