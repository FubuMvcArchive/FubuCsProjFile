using System.IO;
using FubuCore;
using FubuCsProjFile.MSBuild;
using FubuCsProjFile.ProjectFiles.CsProj;
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
        }

        [Test]
        public void smoke_test_creating_a_new_MSBuild_project()
        {
            var project = MSBuildProject.Create<CsProjFile>("MyFoo");
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
            File.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj", true);
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
            File.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj", true);
            var buildProject = MSBuildProject.LoadFrom(fileName);

            buildProject.FindImport(x => x.Project.Contains("Microsoft.CSharp.targets"))
                .ShouldNotBeNull()
                .Project.ShouldEqual(@"$(MSBuildBinPath)\Microsoft.CSharp.targets");

        }

        [Test]
        public void can_add_an_item_group_after_a_specfic_item()
        {
            const string fileName = "SlickGridHarness.csproj";
            File.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj", true);
            var buildProject = MSBuildProject.LoadFrom(fileName);

            var csharpTarget = buildProject.FindImport(x => x.Project.Contains("Microsoft.CSharp.targets"));

            var propGroup = buildProject.AddNewPropertyGroup(insertAfter: csharpTarget).ShouldNotBeNull();
        }
    }
}