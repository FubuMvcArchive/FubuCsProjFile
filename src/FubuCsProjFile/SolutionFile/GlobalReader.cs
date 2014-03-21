using System;
using System.Collections.Generic;

namespace FubuCsProjFile.SolutionFile
{
    public class GlobalReader : ISolutionSectionReader
    {
        public bool Matches(string line)
        {
            return line.Trim() == SolutionConstants.Global;
        }

        public void Read(IEnumerator<string> lines, ISolution solution)
        {
            while (lines.MoveNext() && lines.Current.Trim() != SolutionConstants.EndGlobal)
            {
                if (!lines.Current.Trim().StartsWith(SolutionConstants.GlobalSection))
                {
                    throw new NotSupportedException("Currently only built to handle GlobalSection blocks within the Global section");
                }

                var section = new GlobalSection(lines.Current.Trim());
                solution.Sections.Add(section);

                while (lines.MoveNext() && !lines.Current.Trim().StartsWith(SolutionConstants.EndGlobalSection))
                {
                    section.Read(lines.Current);
                }
            }
        }
    }
}