using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCsProjFile.ProjectFiles.CsProj;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class TemplateGraphLoadingTester
    {
        [SetUp]
        public void SetUp()
        {
            new FileSystem().WriteStringToFile("graph.xml", xml.Replace("'", "\""));
            theGraph = TemplateGraph.Read("graph.xml");
        }

        private string xml = @"
<graph>
<category type='new'>
    <project name='G1' url='http://somewhere.com' description='the G1' template='g1_template' alterations='g1a, g1b' dotnet='v4.5' >
        <option name='Foo' description='the Foo' alterations='e, f, g' url='http://else.com'/>
        <option name='Bar' description='the Bar' alterations='h, i, j' />
    </project>
    <project name='G2' description='the G2' alterations='a, b, c' ></project>
    <project name='Complex' description='the Complicated one' alterations='a, b, c'  >
       <selection name='Select1' description='the selection'>
           <option name='Chiefs' description='the Chiefs' alterations='k, l' url='http://something.com' />
           <option name='Broncos' description='the Broncos' alterations='m, n' />
       </selection>
       <selection name='Select2' description='the 2nd selection'>
           <option name='Foo' description='the Foo' alterations='e, f, g' />
           <option name='Bar' description='the Bar' alterations='h, i, j' />
       </selection>
    </project>
</category>
<category type='add'>
    <project name='G3' description='the G3' template='g3_template' alterations='g1a, g1b' />
    <project name='G4' description='the G4' template='g4_template' alterations='g1a, g1b' />
</category>
</graph>
";
        private TemplateGraph theGraph;

        [Test]
        public void can_associate_options_with_a_generation()
        {
            ProjectTemplate generationType = theGraph.FindCategory("new").FindTemplate("G1");
            generationType.Options.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Foo", "Bar");
        }

        [Test]
        public void reads_dot_net_version_if_attribute_exists()
        {
            var template = theGraph.FindCategory("new").FindTemplate("G1");
            template.DotNetVersion.ShouldEqual(DotNetVersion.V45);
        }

        [Test]
        public void can_find_project_templates_by_category()
        {
            IList<ProjectTemplate> projectTemplates = theGraph.FindCategory("new").Templates;
            projectTemplates.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("G1", "G2", "Complex");
        }

        [Test]
        public void can_load_a_generation_type_without_template()
        {
            ProjectTemplate g = theGraph.FindCategory("new").FindTemplate("G2");
            g.Template.ShouldBeEmpty();
            g.Alterations.ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void can_read_the_url_of_a_project()
        {
            ProjectTemplate g = theGraph.FindCategory("new").FindTemplate("G1");
            g.Url.ShouldEqual("http://somewhere.com");
        }

        [Test]
        public void can_load_a_single_project_template_without_options()
        {
            ProjectTemplate generationType = theGraph.FindCategory("new").FindTemplate("G1");
            generationType.ShouldNotBeNull();
            generationType.Name.ShouldEqual("G1");
            generationType.Template.ShouldEqual("g1_template");
            generationType.Description.ShouldEqual("the G1");
            generationType.Alterations.ShouldHaveTheSameElementsAs("g1a", "g1b");
        }

        // <option name='Foo' description='the Foo' alterations='e, f, g' />

        [Test]
        public void can_read_all_the_properties_of_an_option()
        {
            ProjectTemplate generationType = theGraph.FindCategory("new").FindTemplate("G1");
            Option fooOption = generationType.Options.First();

            fooOption.Description.ShouldEqual("the Foo");
            fooOption.Alterations.ShouldHaveTheSameElementsAs("e", "f", "g");
        }


        /*
    <generation name='Complex' description='the Complicated one' alterations='a, b, c' >
       <selection name='Select1' description='the selection' options='Chiefs, Broncos' />
        <selection name='Select2' description='the 2nd selection' options='Foo, Bar' />
    </generation>

    <option name='Foo' description='the Foo' alterations='e, f, g' />
    <option name='Bar' description='the Bar' alterations='h, i, j' />
    <option name='Chiefs' description='the Chiefs' alterations='k, l' />
    <option name='Broncos' description='the Broncos' alterations='m, n' />
         */

        [Test]
        public void can_read_an_option_selection()
        {
            ProjectTemplate g = theGraph.FindCategory("new").FindTemplate("Complex");
            g.Selections.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Select1", "Select2");
        }

        [Test]
        public void can_read_the_description_for_a_selection()
        {
            ProjectTemplate g = theGraph.FindCategory("new").FindTemplate("Complex");
            OptionSelection select1 = g.Selections.First();

            select1.Description.ShouldEqual("the selection");
        }

        [Test]
        public void can_read_the_options_for_a_selection()
        {
            ProjectTemplate g = theGraph.FindCategory("new").FindTemplate("Complex");
            OptionSelection select1 = g.Selections.First();
            select1.Options.Select(x => x.Name).ShouldHaveTheSameElementsAs("Chiefs", "Broncos");
        }

        [Test]
        public void can_read_the_url_of_an_option()
        {
            ProjectTemplate generationType = theGraph.FindCategory("new").FindTemplate("G1");
            Option fooOption = generationType.Options.First();
            fooOption.Url.ShouldEqual(
                "http://else.com");
        }
    }
}