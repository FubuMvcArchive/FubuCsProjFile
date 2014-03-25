using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuCsProjFile.MSBuild;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing
{

    [TestFixture]
    public class CsProjFileTester
    {
        private FileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {


            fileSystem = new FileSystem();

            fileSystem.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", "FubuMVC.SlickGrid.Docs.csproj");
            fileSystem.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj");
            fileSystem.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness/SlickGridHarness.csproj");


            fileSystem.DeleteDirectory("myproj");
            fileSystem.CreateDirectory("myproj");
        }

        [Test]
        public void creating_a_new_csprojfile_relative_to_a_solution_directory_creates_a_guid()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("Foo", "myproj");
            project.ProjectGuid.ShouldNotEqual(Guid.Empty);

        }

        [Test]
        public void creating_a_new_csprojectfile_create_a_guid()
        {
            var project = CsProjFile.CreateAtLocation("Foo.AssemblyName.csproj", "Foo.AssemblyName");
            project.ProjectGuid.ShouldNotEqual(Guid.Empty);
        }


        [Test]
        public void read_the_dot_net_version_from_csprojfile()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.DotNetVersion.ShouldEqual(DotNetVersion.V40);
        }

        [Test]
        public void write_and_then_read_the_dot_net_version_from_csprojfile()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.DotNetVersion = DotNetVersion.V45;

            project.Save("45Version.xml");

            var copy = CsProjFile.LoadFrom("45Version.xml");
            copy.DotNetVersion.ShouldEqual(DotNetVersion.V45);
        }

        [Test]
        public void creating_a_new_csprojfile_class_library_is_default()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("Foo", "myproj");
            project.ProjectTypes().Single().ShouldEqual(Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"));

        }

        [Test]
        public void read_the_project_guid()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.ProjectGuid.ShouldEqual(Guid.Parse("CACA4EC1-7F9A-4E38-A0A4-94FB4E23B91C"));
        }

        [Test]
        public void read_the_project_name()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.ProjectName.ShouldEqual("FubuMVC.SlickGrid.Docs");
        }

        [Test]
        public void sets_the_assembly_name_and_root_namespace_on_creation()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("Goofy", "Directory");
            project.RootNamespace.ShouldEqual("Goofy");
            project.AssemblyName.ShouldEqual("Goofy");

        }

        [Test]
        public void read_the_project_types_when_it_is_explicit_in_the_project()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj");
            project.ProjectTypes().ShouldHaveTheSameElementsAs(Guid.Parse("349c5851-65df-11da-9384-00065b846f21"), Guid.Parse("fae04ec0-301f-11d3-bf4b-00c04f79efbc"));
        }

        [Test]
        public void read_project_type_as_a_class_library_if_no_explicit_project_type()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.ProjectTypes().Single()
                   .ShouldEqual(Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"));
        }

        [Test]
        public void remove_code_file()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<CodeFile>("foo.cs");
            project.Add<CodeFile>("bar.cs");

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("bar.cs", "foo.cs");

            project2.Remove<CodeFile>("bar.cs");
            project2.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("foo.cs");

            project2.Save();

            var project3 = CsProjFile.LoadFrom(project.FileName);
            project3.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("foo.cs");
        }

        [Test]
        public void remove_code_file_2()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<CodeFile>("foo.cs");
            project.Add<CodeFile>("bar.cs");

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("bar.cs", "foo.cs");

            project2.Remove<CodeFile>(project2.Find<CodeFile>("bar.cs"));
            project2.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("foo.cs");

            project2.Save();

            var project3 = CsProjFile.LoadFrom(project.FileName);
            project3.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("foo.cs");
        }

        // SAMPLE: code-files
        [Test]
        public void add_code_files()
        {
            fileSystem.WriteStringToFile("myproj".AppendPath("foo.cs"), "using System.Web;");
            fileSystem.WriteStringToFile("myproj".AppendPath("bar.cs"), "using System.Web;");

            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<CodeFile>("foo.cs");
            project.Add<CodeFile>("bar.cs");

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("bar.cs", "foo.cs");

            project2.Add<CodeFile>("ten.cs");
            project2.Add<CodeFile>("aaa.cs");

            project2.Save();

            var project3 = CsProjFile.LoadFrom(project2.FileName);
            project3.All<CodeFile>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("aaa.cs", "bar.cs", "foo.cs", "ten.cs");

        }
        // ENDSAMPLE

        [Test]
        public void adding_items_is_idempotent()
        {
            fileSystem.WriteStringToFile("myproj".AppendPath("foo.cs"), "using System.Web;");
            fileSystem.WriteStringToFile("myproj".AppendPath("bar.cs"), "using System.Web;");

            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<CodeFile>("foo.cs");
            project.Add<CodeFile>("bar.cs");

            project.Add<CodeFile>("foo.cs");
            project.Add<CodeFile>("bar.cs");


            project.Add<CodeFile>("bar.cs");
            project.Add<CodeFile>("foo.cs");

            project.All<CodeFile>().Select(x => x.Include).ShouldHaveTheSameElementsAs("bar.cs", "foo.cs");

        }

        [Test]
        public void can_read_embedded_resources()
        {
            // SAMPLE: loading-existing-file
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");

            // Add new references, code items, etc.

            project.Save();
            // ENDSAMPLE

            project = CsProjFile.LoadFrom(project.FileName);

            project.All<EmbeddedResource>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("pak-Config.zip", "pak-Data.zip", "pak-WebContent.zip");
        }

        // SAMPLE: embedded-resources
        [Test]
        public void can_write_embedded_resources()
        {
            fileSystem.WriteStringToFile("myproj".AppendPath("foo.txt"), "using System.Web;");
            fileSystem.WriteStringToFile("myproj".AppendPath("bar.txt"), "using System.Web;");

            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<EmbeddedResource>("foo.txt");
            project.Add<EmbeddedResource>("bar.txt");

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.All<EmbeddedResource>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("bar.txt", "foo.txt");

            project2.Add<EmbeddedResource>("ten.txt");
            project2.Add<EmbeddedResource>("aaa.txt");

            project2.Save();

            var project3 = CsProjFile.LoadFrom(project2.FileName);
            project3.All<EmbeddedResource>().Select(x => x.Include)
                .ShouldHaveTheSameElementsAs("aaa.txt", "bar.txt", "foo.txt", "ten.txt");


        }
        // ENDSAMPLE


        [Test]
        public void can_read_shallow_system_assemblies()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");

            var assemblies = project.All<AssemblyReference>().Select(x => x.Include).ToArray();
            assemblies.ShouldContain("System");
            assemblies.ShouldContain("System.Core");
            assemblies.ShouldContain("System.Data");
            assemblies.ShouldContain("Rhino.Mocks");
        }

        [Test]
        public void can_read_source_control_information()
        {
            var solution = Solution.LoadFrom("FubuMVC.SlickGridTFS.sln");
            var project = solution.Projects.First(item => item.ProjectName == "SlickGridHarness").Project;

            project.SourceControlInformation.ShouldNotBeNull();
            project.SourceControlInformation.ProjectUniqueName.ShouldEqual(@"..\\SlickGridHarness\\SlickGridHarness.csproj");
            project.SourceControlInformation.ProjectName.ShouldEqual(@"../SlickGridHarness");
            project.SourceControlInformation.ProjectLocalPath.ShouldEqual(@"..\\SlickGridHarness");
        }

        [Test]
        public void can_read_source_control_information_when_aux_path_is_specified()
        {
            var solution = Solution.LoadFrom("FubuMVC.SlickGridTFS.Aux.sln");
            var project = solution.Projects.First(item => item.ProjectName == "FubuMVC.SlickGrid").Project;

            project.SourceControlInformation.ShouldNotBeNull();
            project.SourceControlInformation.ProjectUniqueName.ShouldEqual(@"..\\FubuMVC.SlickGrid\\FubuMVC.SlickGrid.csproj");
            project.SourceControlInformation.ProjectName.ShouldEqual(@"../FubuMVC.SlickGrid");
            project.SourceControlInformation.ProjectLocalPath.ShouldEqual(@"..\\FubuMVC.SlickGrid");
        }


        [Test]
        public void can_read_reference_with_hint_path()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");

            var reference = project.Find<AssemblyReference>("Rhino.Mocks");

            reference.HintPath.ShouldEqual(@"..\packages\RhinoMocks\lib\net\Rhino.Mocks.dll");
        }

        [Test]
        public void can_read_then_save_a_change_to_the_include_attribute()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj");

            var contentFile = project.Find<Content>(@"content\scripts\json2.js");
            contentFile.CopyToOutputDirectory.ShouldEqual(ContentCopy.Never);

            // modify
            contentFile.Include = @"content\scripts\json3.js";
            project.Save("MyProj.csproj");

            project = CsProjFile.LoadFrom("MyProj.csproj");
            project.Find<Content>(@"content\scripts\json3.js").ShouldNotBeNull();
        }

        [Test]
        public void can_read_then_change_and_save_a_reference()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");

            var reference = project.Find<AssemblyReference>("Rhino.Mocks");
            reference.HintPath.ShouldEqual(@"..\packages\RhinoMocks\lib\net\Rhino.Mocks.dll");

            // modify
            reference.HintPath = @"..\Modified\RhinoMocks\lib\net\Rhino.Mocks.dll";
            project.Save("MyProj.csproj");

            project = CsProjFile.LoadFrom("MyProj.csproj");
            project.Find<AssemblyReference>("Rhino.Mocks").HintPath.ShouldEqual(@"..\Modified\RhinoMocks\lib\net\Rhino.Mocks.dll");
        }

        [Test]
        public void can_read_then_change_a_reference_and_retrieve_the_modified_item_again()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");

            var reference = project.Find<AssemblyReference>("Rhino.Mocks");
            reference.HintPath.ShouldEqual(@"..\packages\RhinoMocks\lib\net\Rhino.Mocks.dll");

            // modify
            reference.HintPath = @"..\Modified\RhinoMocks\lib\net\Rhino.Mocks.dll";

            // retrieve again
            reference = project.Find<AssemblyReference>("Rhino.Mocks");
            reference.HintPath.ShouldEqual(@"..\Modified\RhinoMocks\lib\net\Rhino.Mocks.dll");
        }

        [Test]
        public void can_read_then_change_and_save_a_code_file()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj");

            var codeFile = project.Find<CodeFile>(@"Simple\SimpleEndpoint.cs");
            codeFile.Link.ShouldBeNull();

            // modify
            codeFile.Link = "SimpleEndpoint.cs";
            project.Save("MyProj.csproj");

            project = CsProjFile.LoadFrom("MyProj.csproj");
            project.Find<CodeFile>(@"Simple\SimpleEndpoint.cs").Link.ShouldEqual(@"SimpleEndpoint.cs");
        }

        [Test]
        public void can_read_then_save_maintains_orginal_node_order()
        {
            // Can be important to maintain orginal node order to minimize merge conflicts checking in
            // changes to a proj file
            var origProj = CsProjFile.LoadFrom("SlickGridHarness.csproj");

            origProj.BuildProject.Settings = MSBuildProjectSettings.MinimizeChanges;
            origProj.Save("MyProj.csproj");

            origProj = CsProjFile.LoadFrom("SlickGridHarness.csproj");
            var newProj = CsProjFile.LoadFrom("MyProj.csproj");

            for (int i = 0; i < origProj.BuildProject.ItemGroups.Count(); i++)
            {
                var origGroup = origProj.BuildProject.ItemGroups.ElementAt(i);
                var newGroup = newProj.BuildProject.ItemGroups.ElementAt(i);

                for (int j = 0; j < origGroup.Items.Count(); j++)
                {
                    var origItem = origGroup.Items.ElementAt(j);
                    var newItem = newGroup.Items.ElementAt(j);

                    origItem.Include.ShouldEqual(newItem.Include);
                }
            }

        }

        [Test]
        public void can_read_then_change_and_save_a_content_item()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj");

            var contentFile = project.Find<Content>(@"content\scripts\json2.js");
            contentFile.CopyToOutputDirectory.ShouldEqual(ContentCopy.Never);

            // modify
            contentFile.CopyToOutputDirectory = ContentCopy.Always;
            project.Save("MyProj.csproj");

            project = CsProjFile.LoadFrom("MyProj.csproj");
            project.Find<Content>(@"content\scripts\json2.js").CopyToOutputDirectory.ShouldEqual(ContentCopy.Always);
        }

        [Test]
        public void can_read_then_change_and_save_a_projecet_reference_item()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj");

            var projectReference = project.Find<ProjectReference>(@"..\FubuMVC.SlickGrid\FubuMVC.SlickGrid.csproj");
            projectReference.ProjectName.ShouldEqual("FubuMVC.SlickGrid");
            projectReference.ProjectGuid.ShouldEqual(new Guid("{A67A0CE1-E4C2-45FC-9019-829D434B2CC4}"));

            // modify
            var newProjectGuid = Guid.NewGuid();
            projectReference.ProjectName = "Bob";
            projectReference.ProjectGuid = newProjectGuid;
            project.Save("MyProj.csproj");

            project = CsProjFile.LoadFrom("MyProj.csproj");
            projectReference = project.Find<ProjectReference>(@"..\FubuMVC.SlickGrid\FubuMVC.SlickGrid.csproj");
            projectReference.ProjectName.ShouldEqual("Bob");
            projectReference.ProjectGuid.ShouldEqual(newProjectGuid);
        }

        [Test]
        public void can_write_system_assemblies()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add<AssemblyReference>("System.Configuration");
            project.Add<AssemblyReference>("System.Security");

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<AssemblyReference>("System.Configuration").ShouldNotBeNull();
            project2.Find<AssemblyReference>("System.Security").ShouldNotBeNull();


        }

        [Test]
        public void can_write_assembly_reference_with_hint_path()
        {
            var hintPath = @"..\packages\RhinoMocks\lib\net\Rhino.Mocks.dll";
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            project.Add(new AssemblyReference("Rhino.Mocks", hintPath));

            project.Save();


            var project2 = CsProjFile.LoadFrom(project.FileName);



            project2.Find<AssemblyReference>("Rhino.Mocks")
                    .HintPath.ShouldEqual(hintPath);
        }

        [Test]
        public void can_write_and_read_project_references()
        {
            var include = @"..\OtherProject\OtherProject.csproj";


            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            var reference1 = new ProjectReference(include)
            {
                ProjectName = "OtherProject",
                ProjectGuid = Guid.NewGuid()
            };

            project.Add(reference1);

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);

            var reference2 = project2.Find<ProjectReference>(reference1.Include);

            var all = project2.All<ProjectReference>();

            reference2.ShouldNotBeNull();
            reference2.ProjectName.ShouldEqual(reference1.ProjectName);
            reference2.ProjectGuid.ShouldEqual(reference1.ProjectGuid);
        }



    }
}