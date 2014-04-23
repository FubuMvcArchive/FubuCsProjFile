using System;
using System.IO;
using FubuCore;
using FubuCsProjFile.MSBuild;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class MSBuildProjectTester
    {
        [SetUp]
        public void SetUp()
        {
            var fileSystem = new FileSystem();
            fileSystem.DeleteDirectory("myfoo");
            fileSystem.CreateDirectory("myfoo");
            fileSystem.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj");
        }

        [Test]
        public void smoke_test_creating_a_new_MSBuild_project()
        {
            var project = MSBuildProject.Create("MyFoo");
            var fileName = "myfoo".AppendPath("MyFoo.csproj");
            project.Save(fileName);

            new MSBuildProject().Load(fileName);
        }

        [Test]
        public void create_from_file()
        {
            var project = MSBuildProject.CreateFromFile("MyBar", Path.Combine("..", "..", "Project.txt"));
            project.Save("MyBar.csproj");

            var file = CsProjFile.LoadFrom("MyBar.csproj");
            file.ProjectName.ShouldEqual("MyBar");
            file.Find<AssemblyReference>("System.Data")
                .ShouldNotBeNull(); // this is in the csproj template in the testing project, but not the embedded one
        }

        [Test]
        public void saving_an_unmodified_project_with_minimize_changes_settings_does_not_update_original_project_file()
        {
            const string fileName = "SlickGridHarness.csproj";
            MSBuildProject.LoadFrom(fileName).Save(fileName);

            var lastWriteTime = File.GetLastWriteTimeUtc(fileName);
            var project = MSBuildProject.LoadFrom(fileName);

            project.Settings = MSBuildProjectSettings.MinimizeChanges;
            project.Save(fileName);

            lastWriteTime.ShouldEqual(File.GetLastWriteTimeUtc(fileName));
        }

        [Test]
        public void can_retrieve_an_import_by_name_as_an_xml_element()
        {
            const string fileName = "SlickGridHarness.csproj";
            var buildProject = MSBuildProject.LoadFrom(fileName);

            buildProject.FindImport(x => x.Project.Contains("Microsoft.CSharp.targets"))
                .ShouldNotBeNull()
                .Project.ShouldEqual(@"$(MSBuildBinPath)\Microsoft.CSharp.targets");

        }

        [Test]
        public void can_add_an_item_group_after_a_specfic_item()
        {
            const string fileName = "SlickGridHarness.csproj";
            var buildProject = MSBuildProject.LoadFrom(fileName);

            var csharpTarget = buildProject.FindImport(x => x.Project.Contains("Microsoft.CSharp.targets"));

            var propGroup = buildProject.AddNewPropertyGroup(insertAfter: csharpTarget).ShouldNotBeNull();
        }

        [Test]
        public void can_read_tools_version()
        {
            var project = MSBuildProject.LoadFrom("SlickGridHarness.csproj");
            project.ToolsVersion.ToString().ShouldEqual("4.0");
        }

        [Test]
        public void can_write_tools_version()
        {
            var project = MSBuildProject.LoadFrom("SlickGridHarness.csproj");
            project.ToolsVersion = new Version(12, 0);
            project.Save("SlickGridHarness.csproj");

            project = MSBuildProject.LoadFrom("SlickGridHarness.csproj");
            project.ToolsVersion.ToString().ShouldEqual("12.0");
        }

        [Test]
        public void can_read_debug_property_group()
        {
            var project = MSBuildProject.LoadFrom("SlickGridHarness.csproj");
            var group = project.GetDebugPropertyGroup();

            group.GetPropertyValue("DefineConstants").ShouldEqual("DEBUG;TRACE");

        }

        [Test]
        public void can_read_release_property_group()
        {
            var project = MSBuildProject.LoadFrom("SlickGridHarness.csproj");
            var group = project.GetReleasePropertyGroup();

            group.GetPropertyValue("DefineConstants").ShouldEqual("TRACE");

        }
    }
}