using System.IO;
using FubuCore;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CodeFileTemplateTester
    {
        private CsProjFile theProject;

        [TestFixtureSetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("Templated");
            new FileSystem().CreateDirectory("Templated");

            theProject = CsProjFile.CreateAtSolutionDirectory("TemplatedProject", "Templated");

        }

        [Test]
        public void get_namespace_shallow()
        {
            CodeFileTemplate.GetNamespace("Foo", "Lib1").ShouldEqual("Lib1");
            CodeFileTemplate.GetNamespace("Foo.cs", "Lib1").ShouldEqual("Lib1");
            CodeFileTemplate.GetNamespace("Sub/Foo.cs", "Lib1").ShouldEqual("Lib1.Sub");
            CodeFileTemplate.GetNamespace("Sub/Foo", "Lib1").ShouldEqual("Lib1.Sub");
            CodeFileTemplate.GetNamespace("Sub/Other/Foo", "Lib1").ShouldEqual("Lib1.Sub.Other");
            CodeFileTemplate.GetNamespace("Sub/Other/Foo.cs", "Lib1").ShouldEqual("Lib1.Sub.Other");
        }

        [Test]
        public void add_simple_class_to_root_of_project()
        {
            CodeFileTemplate.Class("Foo").Alter(theProject);

            var file = "Templated".AppendPath("TemplatedProject", "Foo.cs");
            File.Exists(file).ShouldBeTrue();

            new FileSystem().ReadStringFromFile(file).ShouldEqualWithLineEndings(@"
namespace TemplatedProject
{
    public class Foo
    {

    }
}
".Trim());

            theProject.All<CodeFile>().Any(x => x.Include == "Foo.cs")
                .ShouldBeTrue();
        }


        [Test]
        public void add_deeper_class_to_root_of_project()
        {
            CodeFileTemplate.Class("Bar/Doer").Alter(theProject);

            var file = "Templated".AppendPath("TemplatedProject", "Bar", "Doer.cs");
            File.Exists(file).ShouldBeTrue();

            new FileSystem().ReadStringFromFile(file).ShouldEqualWithLineEndings(@"
namespace TemplatedProject.Bar
{
    public class Doer
    {

    }
}
".Trim());

            theProject.All<CodeFile>().Any(x => x.Include == "Bar/Doer.cs")
                .ShouldBeTrue();
        }


    }
}