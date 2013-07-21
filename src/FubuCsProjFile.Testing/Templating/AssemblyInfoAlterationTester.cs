using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class when_starting_from_an_empty_property_info
    {
        private CsProjFile theProject;
        private string theContents;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var context = TemplatePlan.CreateClean("assembly-info");
            context.Add(new CreateSolution("AssemblyInfoSolution"));

            var project = new ProjectPlan("MyProject");

            context.Add(project);

            var alteration = new AssemblyInfoAlteration("using System.Reflection;", "[assembly: AssemblyTitle(\"%ASSEMBLY_NAME%\")]", "using FubuMVC.Core;", "[assembly: FubuModule]");
            project.Add(alteration);

            context.Execute();

            theProject = CsProjFile.LoadFrom("assembly-info".AppendPath("src", "MyProject", "MyProject.csproj"));
            theContents =
                new FileSystem().ReadStringFromFile("assembly-info".AppendPath("src", "MyProject", "Properties",
                                                                               "AssemblyInfo.cs"));

        }

        [Test]
        public void should_have_added_the_templated_value()
        {
            theContents.ShouldContain("[assembly: AssemblyTitle(\"MyProject\")]");
        }

        [Test]
        public void should_have_added_both_namespaces()
        {
            theContents.ShouldContain("using System.Reflection;");
            theContents.ShouldContain("using FubuMVC.Core;");
        }

        [Test]
        public void should_have_written_the_code_file_to_the_project()
        {
            theProject.Find<CodeFile>("Properties\\AssemblyInfo.cs")
                      .ShouldNotBeNull();
        }
    }


    [TestFixture]
    public class when_starting_from_an_existing_property_info_file
    {
        private CsProjFile theProject;
        private string theContents;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var context = TemplatePlan.CreateClean("assembly-info");
            context.Add(new CreateSolution("AssemblyInfoSolution"));

            var project = new ProjectPlan("MyProject");

            context.Add(project);

            var system = new FileSystem();
            system.CreateDirectory("assembly-info");
            system.CreateDirectory("assembly-info", "src");
            system.CreateDirectory("assembly-info", "src", "MyProject");
            system.CreateDirectory("assembly-info", "src", "MyProject", "Properties");


            var expectedPath = "assembly-info".AppendPath("src", "MyProject", "Properties", "AssemblyInfo.cs");
            system.WriteStringToFile(expectedPath, @"using System.Reflection;

[assembly: AssemblyTitle('MyProject')]
".Replace("'", "\""));

            var alteration = new AssemblyInfoAlteration("using System.Reflection;", "[assembly: AssemblyTitle(\"%ASSEMBLYNAME%\")]", "using FubuMVC.Core;", "[assembly: FubuModule]");
            project.Add(alteration);

            context.Execute();

            theProject = CsProjFile.LoadFrom("assembly-info".AppendPath("src", "MyProject", "MyProject.csproj"));
            theContents =
                new FileSystem().ReadStringFromFile(expectedPath);

        }

        [Test]
        public void should_have_added_the_templated_value()
        {
            theContents.ShouldContain("[assembly: AssemblyTitle(\"MyProject\")]");
        }

        [Test]
        public void should_have_added_both_namespaces()
        {
            theContents.ShouldContain("using System.Reflection;");
            theContents.ShouldContain("using FubuMVC.Core;");
        }

        [Test]
        public void should_have_written_the_code_file_to_the_project()
        {
            theProject.Find<CodeFile>("Properties\\AssemblyInfo.cs")
                      .ShouldNotBeNull();
        }
    }

}