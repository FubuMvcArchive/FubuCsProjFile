using System.Collections.Generic;

namespace FubuCsProjFile.SolutionFile
{
    public interface ISolutionSectionReader
    {
        bool Matches(string line);
        void Read(IEnumerator<string> lines, ISolution solution);
    }
}