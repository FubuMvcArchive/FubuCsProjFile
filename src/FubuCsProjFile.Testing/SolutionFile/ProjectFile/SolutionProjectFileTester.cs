using System;
using FubuCsProjFile.ProjectFiles;
using FubuCsProjFile.SolutionFile.ProjectFiles;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCsProjFile.Testing.SolutionFile.ProjectFile
{
    public class SolutionProjectFileTester : InteractionContext<SolutionProjectFile>
    {
        [Test]
        public void project_is_same_as_from_ctor()
        {
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.FileName).Return("projectfilename");
            var solutionProjectFile = new SolutionProjectFile(project, "directory");
            solutionProjectFile.Project.ShouldBeTheSameAs(project);
        }

        [Test]
        public void project_type_comes_from_project_file_when_created_from_IProjectFile()
        {
            var type = Guid.NewGuid();
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.Type).Return(type);
            project.Stub(x => x.FileName).Return("projectfilename");

            var solutionProjectFile = new SolutionProjectFile(project, "directory");

            solutionProjectFile.Type.ShouldEqual(type);
        }

        [Test]
        public void project_type_comes_from_solution_value_when_IProjectFile_has_not_been_loaded()
        {
            var type = Guid.NewGuid();
            var solutionProjectFile = new SolutionProjectFile(type, Guid.NewGuid(), "projname", "relpath", () =>
            {
                throw new Exception("Should not attempt to create project in this test");
            });
            solutionProjectFile.Type.ShouldEqual(type);
        }

        [Test]
        public void project_type_comes_from_project_file_when_loaded()
        {
            var type = Guid.NewGuid();
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.Type).Return(type);

            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), Guid.NewGuid(), "projname", "relpath", () => project);
            solutionProjectFile.Project.ShouldBeTheSameAs(project);

            solutionProjectFile.Type.ShouldEqual(type);
        }

        [Test]
        public void does_not_save_project_if_not_loaded()
        {
            var project = MockFor<IProjectFile>();

            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), Guid.NewGuid(), "projname", "relpath", () => project);
            solutionProjectFile.Save();

            project.AssertWasNotCalled(x => x.Save());
        }

        [Test]
        public void saves_project_if_loaded()
        {
            var project = MockFor<IProjectFile>();

            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), Guid.NewGuid(), "projname", "relpath", () => project);
            solutionProjectFile.Project.ShouldBeTheSameAs(project);
            solutionProjectFile.Save();

            project.AssertWasCalled(x => x.Save());
        }
    }
}