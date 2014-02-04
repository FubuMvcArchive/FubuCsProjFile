using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class NoneTester
    {
        [Test]
        public void can_add_a_file_with_build_action_none()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("MyProj", "myproj");
            var content = new None("packages.config");

            project.Add(content);

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<None>("packages.config")
                .CopyToOutputDirectory.ShouldEqual(ContentCopy.Never);
        }

        [Test]
        public void can_read_existing_items_with_build_action_set_to_none()
        {
            var project = CsProjFile.LoadFrom("SlickGridHarness.csproj.fake");

            project.Find<None>(@"Paging\Paged.spark").ShouldNotBeNull();
        }
    }
}