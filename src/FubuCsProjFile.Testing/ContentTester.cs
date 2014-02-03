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
    }
}