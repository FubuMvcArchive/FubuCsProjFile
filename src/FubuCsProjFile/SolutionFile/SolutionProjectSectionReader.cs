using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCsProjFile.SolutionFile.ProjectFiles;
using FubuCsProjFile.SolutionFile.SolutionItems;

namespace FubuCsProjFile.SolutionFile
{
    public class SolutionProjectSectionReader : ISolutionSectionReader
    {
        private static readonly IList<ISolutionProjectSectionReaderFactory> Factories = new ISolutionProjectSectionReaderFactory[]
        {
            new SolutionFolderSectionReaderFactory(),
            new SolutionProjectFileReaderFactory() 
        };

        public bool Matches(string line)
        {
            return line.StartsWith(SolutionConstants.Project);
        }

        public void Read(IEnumerator<string> lines, ISolution solution)
        {
            var line = lines.Current;

            var parts =  line.ToDelimitedArray('=');
            var projectType = Guid.Parse(parts.First().TextBetweenSquiggles());
            var projectGuid = Guid.Parse(parts.Last().TextBetweenSquiggles());

            var secondParts = parts.Last().ToDelimitedArray();
            var projectName = secondParts.First().TextBetweenQuotes();
            var relativePath = secondParts.ElementAt(1).TextBetweenQuotes().Replace("\\", "/"); // Windows is forgiving

            var factory = Factories.First(x => x.Matches(projectType));

            var projectReader = factory.Build(projectType, projectGuid, projectName, relativePath, solution);

            while (lines.MoveNext() && !lines.Current.StartsWith(SolutionConstants.EndProject))
            {
                projectReader.Read(lines.Current);
            }
        }
    }

    public interface ISolutionProjectSectionReaderFactory
    {
        bool Matches(Guid type);
        ISolutionProjectReader Build(Guid projectType, Guid projectGuid, string projectName, string relativePath, ISolution solution);
    }

    public interface ISolutionProjectReader
    {
        void Read(string line);
    }
}