using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCsProjFile.MSBuild;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap.Configuration;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class AssemblyReferenceTester
    {
        [SetUp]
        public void SetUp()
        {            
            var fileSystem = new FileSystem();

            fileSystem.Copy("FubuMVC.SlickGrid.Docs.csproj.fake", "FubuMVC.SlickGrid.Docs.csproj");
            fileSystem.Copy("SlickGridHarness.csproj.fake", "SlickGridHarness.csproj");
        }
        [Test]
        public void specific_version_should_serialize_using_boolean_string_that_matches_visual_studio_behaviour()
        {

            var reference = new AssemblyReference("log4net");
            var element = new XmlDocument().CreateElement("Reference");
            reference.Configure(new MSBuildItemGroup(new MSBuildProject(), element));

            reference.SpecificVersion = false;
            reference.Save();

            element.GetElementsByTagName("SpecificVersion")[0].InnerText.ShouldEqual("False"); // and not "false"
        }

        [Test]
        public void can_retrieve_the_assembly_name_when_hint_path_is_is_specified()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.All<AssemblyReference>().Any(assembly => assembly.AssemblyName.Equals("nunit.framework.dll")).ShouldBeTrue();
        }

        [Test]
        public void can_retrieve_the_assembly_name()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.All<AssemblyReference>().Any(assembly => assembly.AssemblyName.Equals("System.Data.DataSetExtensions.dll")).ShouldBeTrue();
        }

        [Test]
        public void can_retrieve_the_assembly_name_when_include_contains_version_info()
        {
            var project = CsProjFile.LoadFrom("FubuMVC.SlickGrid.Docs.csproj");
            project.All<AssemblyReference>().Any(assembly => assembly.AssemblyName.Equals("Newtonsoft.Json.dll")).ShouldBeTrue();
        }
    }
}