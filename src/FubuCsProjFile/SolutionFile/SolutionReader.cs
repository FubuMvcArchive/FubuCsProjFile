using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.SolutionFile
{
    public static class SolutionReader
    {
        private static readonly IList<ISolutionSectionReader> Readers = new List<ISolutionSectionReader>
        {
            new VersionSectionReader(),
            new SolutionProjectSectionReader(),
            new GlobalReader()
        };

        public static ISolution LoadFrom(string filename)
        {
            return LoadFrom(filename, new FileSystem());
        }

        public static ISolution LoadFrom(string filename, IFileSystem fileSystem)
        {
            if (!fileSystem.FileExists(filename))
            {
                throw new FileNotFoundException("Could not find the solution file", filename);
            }

            var lines = fileSystem.ReadStringFromFile(filename).SplitOnNewLine();
            return Read(filename, lines);
        }

        public static ISolution Read(string fileName, IEnumerable<string> lines)
        {
            return Read(fileName, lines.GetEnumerator());
        }

        private static ISolution Read(string fileName, IEnumerator<string> lines)
        {
            var solution = new Solution(fileName);

            while (lines.MoveNext() && lines.Current.Trim().IsNotEmpty())
            {
                var reader = Readers.First(x => x.Matches(lines.Current));
                reader.Read(lines, solution);
            }

            return solution;
        }
    }
}