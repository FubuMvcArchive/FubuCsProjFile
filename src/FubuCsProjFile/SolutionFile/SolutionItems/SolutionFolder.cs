using System;
using System.Collections.Generic;
using System.IO;

namespace FubuCsProjFile.SolutionFile.SolutionItems
{
    public class SolutionFolder : SolutionProject
    {
        public static readonly Guid TypeId = new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8");

        public override Guid Type { get { return TypeId; } }

        public SolutionFolder(Guid projectGuid, string projectName, string relativePath) : base(projectGuid, projectName, relativePath)
        {
            // TODO: Change this so we can configure the solution folder contents
            RawLines = new List<string>();
        }

        public IList<string> RawLines { get; private set; }

        protected override void ProjectDataForSolutionFile(StringWriter writer)
        {
            RawLines.Each(line => writer.WriteLine(line));
        }
    }
}