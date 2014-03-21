using System;
using System.Collections.Generic;
using System.Linq;
using FubuCsProjFile.SolutionFile;
using FubuCsProjFile.SolutionFile.ProjectFiles;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCsProjFile.Testing.SolutionFile.ProjectFile
{
    public class SolutionProjectFileReaderFactoryTester : InteractionContext<SolutionProjectFileReaderFactory>
    {
        private ISolution _solution;
        private List<ISolutionProject> _projects;

        protected override void beforeEach()
        {
            _projects = new List<ISolutionProject>();
            _solution = MockFor<ISolution>();
            _solution.Stub(x => x.Projects).Return(_projects);
        }

        [Test]
        public void MatchesAnyGuid()
        {
            ClassUnderTest.Matches(Guid.NewGuid()).ShouldBeTrue();
        }

        [Test]
        public void BuildReturnsNoOpReader()
        {
            var reader = ClassUnderTest.Build(Guid.NewGuid(), Guid.NewGuid(), "projectName", "relpath", _solution);
            reader.ShouldNotBeNull();
            reader.ShouldBeOfType<NoOpSolutionProjectReader>();
        }

        [Test]
        public void BuildAddsProjectToSolution()
        {
            _projects.OfType<SolutionProjectFile>().Count().ShouldEqual(0);
            ClassUnderTest.Build(Guid.NewGuid(), Guid.NewGuid(), "projectName", "relpath", _solution);
            _projects.OfType<SolutionProjectFile>().Count().ShouldEqual(1);
        }
    }
}