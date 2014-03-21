using System;
using System.Collections.Generic;

namespace FubuCsProjFile.SolutionFile
{
    public class GlobalReader : ISolutionSectionReader
    {
        public bool Matches(string line)
        {
            return line.Trim() == "Global";
        }

        public void Read(IEnumerator<string> lines, ISolution solution)
        {
            while (lines.MoveNext() && lines.Current.Trim() != "EndGlobal")
            {
                if (!lines.Current.Trim().StartsWith("GlobalSection"))
                {
                    throw new NotImplementedException("Currently only built to handle GlobalSection blocks within the Global section");
                }

                var section = new GlobalSection(lines.Current.Trim());
                solution.Sections.Add(section);

                while (lines.MoveNext() && !lines.Current.Trim().StartsWith("EndGlobalSection"))
                {
                    section.Read(lines.Current);
                }
            }
        }
    }
}