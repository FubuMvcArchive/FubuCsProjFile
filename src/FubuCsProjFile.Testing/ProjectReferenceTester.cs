using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class reading_a_project_reference_from_text
    {
        private SolutionProject theSolutionProject;
        
        [SetUp]
        public void SetUp()
        {
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory("Solution1");
            fileSystem.CleanDirectory("Solution1");

            fileSystem.CreateDirectory(@"Solution1\docs");
            fileSystem.CreateDirectory(@"Solution1\harness");
            fileSystem.Copy("SlickGridHarness.csproj.fake", @"Solution1\harness\SlickGridHarness.csproj");
            fileSystem.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", @"Solution1\docs\FubuMVC.SlickGrid.Docs.csproj");
            
            theSolutionProject = new SolutionProject("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"FubuMVC.SlickGrid\", \"FubuMVC.SlickGrid\\FubuMVC.SlickGrid.csproj\", \"{A67A0CE1-E4C2-45FC-9019-829D434B2CC4}\"", "Solution1");
        }

        [Test]
        public void can_load_the_project_on_demand_when_it_does_not_exist()
        {
            theSolutionProject.Project.ShouldNotBeNull();
            theSolutionProject.Project.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
            theSolutionProject.Project.FileName.ToFullPath()
                        .ShouldEqual(
                            "Solution1".AppendPath("FubuMVC.SlickGrid", "FubuMVC.SlickGrid.csproj").ToFullPath());
        }

        [Test]
        public void the_project_guid()
        {
            theSolutionProject.ProjectGuid.ShouldEqual(Guid.Parse("A67A0CE1-E4C2-45FC-9019-829D434B2CC4"));
        }

        [Test]
        public void the_reference_type()
        {
            theSolutionProject.ProjectType.ShouldEqual(Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"));
        }

        [Test]
        public void the_project_name()
        {
            theSolutionProject.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
        }

        [Test]
        public void the_relative_path()
        {
            theSolutionProject.RelativePath.ShouldEqual("FubuMVC.SlickGrid/FubuMVC.SlickGrid.csproj");
        }

        [Test]
        public void creating_a_project_references_from_a_project_copies_attributes()
        {
            var sourceProject = new CsProjFile(@"Solution1\harness\SlickGridHarness.csproj");
            var targetProject = new CsProjFile(@"Solution1\docs\FubuMVC.SlickGrid.Docs.csproj");
            var projectReference = new ProjectReference(targetProject, sourceProject);

            projectReference.Include.ShouldEqual(@"..\harness\SlickGridHarness.csproj");
            projectReference.ProjectGuid.ShouldEqual(sourceProject.ProjectGuid);
            projectReference.ProjectName.ShouldEqual(sourceProject.ProjectName);
        }
    }
}