using FubuCsProjFile.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class TextFileTester
    {
        [Test]
        public void read_contents()
        {
            TextFile.FileSystem.WriteStringToFile("foo.txt", "a bunch of text");

            var file = new TextFile("foo.txt");
            file.ReadAll().ShouldEqual("a bunch of text");
        }

        [Test]
        public void read_lines()
        {
            TextFile.FileSystem.WriteStringToFile("bar.txt", @"
a
b
c
d
e
f
".Trim());

            var file = new TextFile("bar.txt");
            file.ReadLines().ShouldHaveTheSameElementsAs("a", "b", "c", "d", "e", "f");
        }
    }
}