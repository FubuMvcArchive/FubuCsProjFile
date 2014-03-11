using System.IO;
using FubuCore;
using FubuCsProjFile.ProjectFiles;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CodeFileTemplateTester
    {
        private IProjectFile theProject;
        private ProjectPlan thePlan;

        [TestFixtureSetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("Templated");
            new FileSystem().CreateDirectory("Templated");

            theProject = ProjectCreator.CreateAtSolutionDirectory("TemplatedProject", "Templated", ProjectType.CsProj);
            thePlan = new ProjectPlan(theProject.ProjectName);
        }



        [Test]
        public void add_simple_class_to_root_of_project()
        {
            CodeFileTemplate.Class("Foo").Alter(theProject, thePlan);

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
        public void add_simple_class_that_has_substitutions_on_its_name()
        {
            thePlan.Substitutions.Set("%SHORT_NAME%", "MyFoo");

            CodeFileTemplate.Class("%SHORT_NAME%Registry").Alter(theProject, thePlan);

            var file = "Templated".AppendPath("TemplatedProject", "MyFooRegistry.cs");
            File.Exists(file).ShouldBeTrue();

            new FileSystem().ReadStringFromFile(file).ShouldEqualWithLineEndings(@"
namespace TemplatedProject
{
    public class MyFooRegistry
    {

    }
}
".Trim());

            theProject.All<CodeFile>().Any(x => x.Include == "MyFooRegistry.cs")
                .ShouldBeTrue();
        }


        [Test]
        public void add_deeper_class_to_root_of_project()
        {
            CodeFileTemplate.Class(Path.Combine ("Bar", "Doer")).Alter(theProject, thePlan);

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

            theProject.All<CodeFile>().Any(x => x.Include == "Bar\\Doer.cs")
                .ShouldBeTrue();
        }


    }
}