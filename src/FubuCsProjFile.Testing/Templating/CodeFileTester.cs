using System;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CodeFileTester
    {
        [Test]
        public void read_and_write_with_a_link()
        {
            var project = CsProjFile.CreateAtSolutionDirectory("Foo", Guid.NewGuid().ToString());

            var file = new CodeFile("../CommonAssemblyInfo.cs") {Link = "CommonAssemblyInfo.cs"};

            project.Add(file);

            project.Save();

            var project2 = CsProjFile.LoadFrom(project.FileName);
            project2.Find<CodeFile>("../CommonAssemblyInfo.cs")
                    .Link.ShouldEqual("CommonAssemblyInfo.cs");
        }
    }
}