using System;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class reading_a_project_reference_from_text
    {
        private ProjectReference theProject;

        [SetUp]
        public void SetUp()
        {
            theProject = new ProjectReference("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"FubuMVC.SlickGrid\", \"FubuMVC.SlickGrid\\FubuMVC.SlickGrid.csproj\", \"{A67A0CE1-E4C2-45FC-9019-829D434B2CC4}\"");
        }

        [Test]
        public void the_project_guid()
        {
            theProject.ProjectGuid.ShouldEqual(Guid.Parse("A67A0CE1-E4C2-45FC-9019-829D434B2CC4"));
        }

        [Test]
        public void the_reference_type()
        {
            theProject.ProjectType.ShouldEqual(Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"));
        }

        [Test]
        public void the_project_name()
        {
            theProject.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
        }

        [Test]
        public void the_relative_path()
        {
            theProject.RelativePath.ShouldEqual("FubuMVC.SlickGrid/FubuMVC.SlickGrid.csproj");
        }
    }
}