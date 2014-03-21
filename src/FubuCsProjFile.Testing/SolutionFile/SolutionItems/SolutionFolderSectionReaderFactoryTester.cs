using System;
using System.Collections.Generic;
using System.Linq;
using FubuCsProjFile.SolutionFile;
using FubuCsProjFile.SolutionFile.SolutionItems;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCsProjFile.Testing.SolutionFile.SolutionItems
{
    public class SolutionFolderSectionReaderFactoryTester : InteractionContext<SolutionFolderSectionReaderFactory>
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
        public void matches()
        {
            new SolutionFolderSectionReaderFactory().Matches(SolutionFolder.TypeId).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_random_guid()
        {
            new SolutionFolderSectionReaderFactory().Matches(Guid.NewGuid()).ShouldBeFalse();
        }

        [Test]
        public void does_not_match_empty_guid()
        {
            new SolutionFolderSectionReaderFactory().Matches(Guid.Empty).ShouldBeFalse();
        }

        [Test]
        public void creates_solution_folder_section_reader()
        {
            var reader = new SolutionFolderSectionReaderFactory().Build(Guid.NewGuid(), Guid.NewGuid(), "projname", "relpath", _solution);
            reader.ShouldNotBeNull();
            reader.ShouldBeOfType<SolutionFolderSectionReader>();
        }

        [Test]
        public void creates_and_adds_solution_folder_with_project_guid()
        {
            var projectGuid = Guid.NewGuid();
            new SolutionFolderSectionReaderFactory().Build(Guid.NewGuid(), projectGuid, "projname", "relpath", _solution);
            _solution.Projects.Single().ProjectGuid.ShouldEqual(projectGuid);
        }

        [Test]
        public void creates_and_adds_solution_folder_with_proj_name()
        {
            const string name = "project name";
            new SolutionFolderSectionReaderFactory().Build(Guid.NewGuid(), Guid.NewGuid(), name, "relpath", _solution);
            _solution.Projects.Single().ProjectName.ShouldEqual(name);
        }

        [Test]
        public void creates_and_adds_solution_folder_with_relative_path()
        {
            const string path = "relative path";
            new SolutionFolderSectionReaderFactory().Build(Guid.NewGuid(), Guid.NewGuid(), "projname", path, _solution);
            _solution.Projects.Single().RelativePath.ShouldEqual(path);
        }
    }
}