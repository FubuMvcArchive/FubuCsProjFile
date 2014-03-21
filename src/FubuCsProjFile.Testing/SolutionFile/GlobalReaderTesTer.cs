using System;
using System.Collections.Generic;
using System.Linq;
using FubuCsProjFile.SolutionFile;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCsProjFile.Testing.SolutionFile
{
    public class GlobalReaderTester : InteractionContext<GlobalReader>
    {
        [TestCase("Global", Result = true)]
        [TestCase(" Global", Result = true)]
        [TestCase("Global ", Result = true)]
        [TestCase(" Global ", Result = true)]
        [TestCase(" EndGlobal ", Result = false)]
        [TestCase(" GlobalSection ", Result = false)]
        public bool matches(string line)
        {
            return new GlobalReader().Matches(line);
        }

        [Test]
        public void reads_global_sections()
        {
            var lines =
@"Global
    GlobalSection(SectionName1) = preSolution
        test1
        test2
    EndGlobalSection
    GlobalSection(SectionName2) = postSolution
        test3
        test4
    EndGlobalSection
EndGlobal".SplitOnNewLine().AsEnumerable();

            var solution = MockFor<ISolution>();
            solution.Stub(x => x.Sections).Return(new List<GlobalSection>());
            var enumerator = lines.GetEnumerator();
            enumerator.MoveNext();

            new GlobalReader().Read(enumerator, solution);
            solution.Sections.Count.ShouldEqual(2);

            solution.Sections[0].Properties.ShouldHaveTheSameElementsAs("test1", "test2");
            solution.Sections[1].Properties.ShouldHaveTheSameElementsAs("test3", "test4");
        }

        [Test]
        public void throws_on_unknown_sections()
        {
            var lines =
@"Global
    OtherSection
    EndOtherSection
EndGlobal".SplitOnNewLine().AsEnumerable();

            var solution = MockFor<ISolution>();
            solution.Stub(x => x.Sections).Return(new List<GlobalSection>());
            var enumerator = lines.GetEnumerator();
            enumerator.MoveNext();

            Exception<NotSupportedException>.ShouldBeThrownBy(() => new GlobalReader().Read(enumerator, solution));
        }
    }
}