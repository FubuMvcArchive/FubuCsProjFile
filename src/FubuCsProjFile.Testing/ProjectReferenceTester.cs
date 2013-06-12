using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class reading_a_project_reference_from_text
    {
        private ProjectReference theReference;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().CreateDirectory("Solution1");
            new FileSystem().CleanDirectory("Solution1");

            theReference = new ProjectReference("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"FubuMVC.SlickGrid\", \"FubuMVC.SlickGrid\\FubuMVC.SlickGrid.csproj\", \"{A67A0CE1-E4C2-45FC-9019-829D434B2CC4}\"", "Solution1");
        }

        [Test]
        public void can_load_the_project_on_demand_when_it_does_not_exist()
        {
            theReference.Project.ShouldNotBeNull();
            theReference.Project.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
            theReference.Project.FileName.ToFullPath()
                        .ShouldEqual(
                            "Solution1".AppendPath("FubuMVC.SlickGrid", "FubuMVC.SlickGrid.csproj").ToFullPath());
        }

        [Test]
        public void the_project_guid()
        {
            theReference.ProjectGuid.ShouldEqual(Guid.Parse("A67A0CE1-E4C2-45FC-9019-829D434B2CC4"));
        }

        [Test]
        public void the_reference_type()
        {
            theReference.ProjectType.ShouldEqual(Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"));
        }

        [Test]
        public void the_project_name()
        {
            theReference.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
        }

        [Test]
        public void the_relative_path()
        {
            theReference.RelativePath.ShouldEqual("FubuMVC.SlickGrid/FubuMVC.SlickGrid.csproj");
        }
    }
}