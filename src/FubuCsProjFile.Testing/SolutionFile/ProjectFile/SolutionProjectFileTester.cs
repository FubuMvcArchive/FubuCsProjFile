using System;
using System.IO;
using FubuCsProjFile.ProjectFiles;
using FubuCsProjFile.SolutionFile;
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
        public void project_guid_is_same_as_from_ctor_project_file()
        {
            var projectGuid = Guid.NewGuid();
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.ProjectGuid).Return(projectGuid);
            project.Stub(x => x.FileName).Return("projectfilename");
            var solutionProjectFile = new SolutionProjectFile(project, "directory");
            solutionProjectFile.ProjectGuid.ShouldEqual(projectGuid);
        }

        [Test]
        public void project_name_is_same_as_from_ctor_project_file()
        {
            const string name = "SomeName";
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.ProjectName).Return(name);
            project.Stub(x => x.FileName).Return("projectfilename");
            var solutionProjectFile = new SolutionProjectFile(project, "directory");
            solutionProjectFile.ProjectName.ShouldEqual(name);
        }

        [Test]
        public void project_relative_path_is_same_as_from_ctor_project_file()
        {
            const string solutionDir = "directory";
            var path = Path.Combine("project", "project.proj");
            var fullPath = Path.Combine(solutionDir, path);
            var project = MockFor<IProjectFile>();
            project.Stub(x => x.FileName).Return(fullPath);
            var solutionProjectFile = new SolutionProjectFile(project, solutionDir);
            solutionProjectFile.RelativePath.ShouldEqual(path);
        }

        [Test]
        public void project_guid_is_same_as_from_ctor()
        {
            var projectGuid = Guid.NewGuid();
            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), projectGuid, "projname", "relpath", (ISolution) null);
            solutionProjectFile.ProjectGuid.ShouldEqual(projectGuid);
        }

        [Test]
        public void project_name_is_same_as_from_ctor()
        {
            const string name = "SomeName";
            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), Guid.NewGuid(), name, "relpath", (ISolution) null);
            solutionProjectFile.ProjectName.ShouldEqual(name);
        }

        [Test]
        public void project_relative_path_is_same_as_from_ctor()
        {
            var relpath = Path.Combine("directory", "project", "project.proj");
            var solutionProjectFile = new SolutionProjectFile(Guid.NewGuid(), Guid.NewGuid(), "projname", relpath, (ISolution) null);
            solutionProjectFile.RelativePath.ShouldEqual(relpath);
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

        [Test]
        public void project_for_solution()
        {
            var projectType = Guid.Parse("CE9D0F6E-52E8-498F-9570-AF8D2E79264B");
            var projectGuid = Guid.Parse("773048E8-7A48-4255-BA0B-D4308C850AFF");
            const string name = "TestProjectName";
            const string relpath = @"project\project.proj";
            var solutionProjectFile = new SolutionProjectFile(projectType, projectGuid, name, relpath, (ISolution) null);

            var writer = new StringWriter();
            solutionProjectFile.ForSolutionFile(writer);
            writer.ToString().ShouldEqual("Project(\"{CE9D0F6E-52E8-498F-9570-AF8D2E79264B}\") = \"TestProjectName\", \"project\\project.proj\", \"{773048E8-7A48-4255-BA0B-D4308C850AFF}\"\r\nEndProject\r\n");
        }
    }
}