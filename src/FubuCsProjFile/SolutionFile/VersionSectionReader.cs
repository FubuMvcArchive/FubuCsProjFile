using System.Collections.Generic;
using System.Linq;

namespace FubuCsProjFile.SolutionFile
{
    public class VersionSectionReader : ISolutionSectionReader
    {
        public bool Matches(string line)
        {
            return SolutionFileVersioning.VersionLines.Any(kvp => kvp.Value[0].Equals(line));
        }

        public void Read(IEnumerator<string> lines, ISolution solution)
        {
            solution.Version = SolutionFileVersioning.VersionLines
                .First(x => x.Value.First() == lines.Current)
                .Key;

            var versionLineCount = SolutionFileVersioning.VersionLines[solution.Version].Length;

            for (var i = 0; i < versionLineCount - 1; i++)
            {
                if (!lines.MoveNext())
                    return;
            }
        }
    }
}