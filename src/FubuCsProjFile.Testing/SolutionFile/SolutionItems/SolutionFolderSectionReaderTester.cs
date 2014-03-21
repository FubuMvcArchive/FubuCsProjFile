using System;
using FubuCsProjFile.SolutionFile.SolutionItems;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.SolutionFile.SolutionItems
{
    public class SolutionFolderSectionReaderTester
    {
        [Test]
        public void read_simply_adds_line_to_raw_lines()
        {
            var folder = new SolutionFolder(Guid.NewGuid(), "projname", "relpath");
            var reader = new SolutionFolderSectionReader(folder);

            folder.RawLines.ShouldHaveTheSameElementsAs();
            reader.Read("something");
            folder.RawLines.ShouldHaveTheSameElementsAs("something");
            reader.Read("something new");
            folder.RawLines.ShouldHaveTheSameElementsAs("something", "something new");
        }
    }
}