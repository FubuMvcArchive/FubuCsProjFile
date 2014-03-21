using System;
using System.IO;
using FubuCsProjFile.SolutionFile.SolutionItems;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.SolutionFile.SolutionItems
{
    public class SolutionFolderTester
    {
        [Test]
        public void project_guid_is_same_as_from_ctor()
        {
            var projectGuid = Guid.NewGuid();
            var solutionProjectFile = new SolutionFolder(projectGuid, "projname", "relpath");
            solutionProjectFile.ProjectGuid.ShouldEqual(projectGuid);
        }

        [Test]
        public void project_name_is_same_as_from_ctor()
        {
            const string name = "SomeName";
            var solutionProjectFile = new SolutionFolder(Guid.NewGuid(), name, "relpath");
            solutionProjectFile.ProjectName.ShouldEqual(name);
        }

        [Test]
        public void project_relative_path_is_same_as_from_ctor()
        {
            var relpath = "Something Random";
            var solutionProjectFile = new SolutionFolder(Guid.NewGuid(), "projname", relpath);
            solutionProjectFile.RelativePath.ShouldEqual(relpath);
        }

        [Test]
        public void project_type_is_set()
        {
            var solutionProjectFile = new SolutionFolder(Guid.NewGuid(), "projname", "relpath");
            solutionProjectFile.Type.ShouldEqual(SolutionFolder.TypeId);
        }

        [Test]
        public void project_for_solution_no_raw_lines()
        {
            var projectGuid = Guid.Parse("773048E8-7A48-4255-BA0B-D4308C850AFF");
            const string name = "Test Something";
            var solutionProjectFile = new SolutionFolder(projectGuid, name, name);

            var writer = new StringWriter();
            solutionProjectFile.ForSolutionFile(writer);
            writer.ToString().ShouldEqual("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Test Something\", \"Test Something\", \"{773048E8-7A48-4255-BA0B-D4308C850AFF}\"\r\nEndProject\r\n");
        }

        [Test]
        public void project_for_solution_with_raw_lines()
        {
            var projectGuid = Guid.Parse("773048E8-7A48-4255-BA0B-D4308C850AFF");
            const string name = "Test Something";
            var solutionProjectFile = new SolutionFolder(projectGuid, name, name);
            solutionProjectFile.RawLines.Add("\t\tTest1 = Test1");
            solutionProjectFile.RawLines.Add("\t\tTest2 = Test2");

            var writer = new StringWriter();
            solutionProjectFile.ForSolutionFile(writer);
            writer.ToString().ShouldEqual("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Test Something\", \"Test Something\", \"{773048E8-7A48-4255-BA0B-D4308C850AFF}\"\r\n\t\tTest1 = Test1\r\n\t\tTest2 = Test2\r\nEndProject\r\n");
        }
    }
}