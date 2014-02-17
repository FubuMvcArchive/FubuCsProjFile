using System.Xml;
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
    }
}