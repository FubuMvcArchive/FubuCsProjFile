using System.IO;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class ContentTester
    {
        [Test]
        public void default_copy_to_behavior_is_never()
        {
            new Content("anything").CopyToOutputDirectory
                .ShouldEqual(ContentCopy.Never);
        }

        [Test]
        public void can_add_and_load_never_copy_content()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            var content = new Content("Something.txt")
            {
                CopyToOutputDirectory = ContentCopy.Never
            };

            project.Add(content);

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<Content>("Something.txt")
                .CopyToOutputDirectory.ShouldEqual(ContentCopy.Never);
        }

        [Test]
        public void can_add_and_load_always_copy_content()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            var content = new Content("Something.txt")
            {
                CopyToOutputDirectory = ContentCopy.Always
            };

            project.Add(content);

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<Content>("Something.txt")
                .CopyToOutputDirectory.ShouldEqual(ContentCopy.Always);
        }


        [Test]
        public void can_add_and_load_IfNewer_copy_content()
        {
            // SAMPLE: adding-content-to-csprojfile
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            var content = new Content("Something.txt")
            {
                CopyToOutputDirectory = ContentCopy.IfNewer
            };

            project.Add(content);
            // ENDSAMPLE

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<Content>("Something.txt")
                .CopyToOutputDirectory.ShouldEqual(ContentCopy.IfNewer);
        }

        [Test]
        public void read_link_element_value()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj.fake");
            project.Find<Content>(@"..\..\..\Config\App.config").Link.ShouldEqual("App.config");
        }

        [Test]
        public void write_link_element_value()
        {
            File.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", "FubuMVC.SlickGrid.Docs.csproj", true);
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.Find<Content>(@"..\..\..\Config\App.config").Link = "App.exe.config";
            project.Save();

            project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.Find<Content>(@"..\..\..\Config\App.config").Link.ShouldEqual("App.exe.config");
        }
    }
}