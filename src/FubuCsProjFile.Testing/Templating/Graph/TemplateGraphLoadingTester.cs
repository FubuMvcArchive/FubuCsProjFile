using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class TemplateGraphLoadingTester
    {
        private string xml = @"
<graph>
    <generation name='G1' description='the G1' template='g1_template' alterations='g1a, g1b' tags='x, y, z' options='Foo, Bar'></generation>
    <generation name='G2' description='the G2' alterations='a, b, c' ></generation>
    <generation name='Complex' description='the Complicated one' alterations='a, b, c' tags='x' >
       <selection name='Select1' description='the selection' options='Chiefs, Broncos' />
       <selection name='Select2' description='the 2nd selection' options='Foo, Bar' />
    </generation>

    <option name='Foo' description='the Foo' alterations='e, f, g' />
    <option name='Bar' description='the Bar' alterations='h, i, j' />
    <option name='Chiefs' description='the Chiefs' alterations='k, l' />
    <option name='Broncos' description='the Broncos' alterations='m, n' />
</graph>
";
        private TemplateGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().WriteStringToFile("graph.xml", xml.Replace("'", "\""));
            theGraph = TemplateGraph.Read("graph.xml");
        }

        [Test]
        public void can_find_generation_types_by_tag()
        {
            theGraph.TemplateSetsForTag("x").Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("G1", "Complex");

            theGraph.TemplateSetsForTag("X").Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("G1", "Complex");

        }

        [Test]
        public void can_load_a_single_generation_type_without_options()
        {
            var generationType = theGraph.TemplateSetFor("G1");
            generationType.ShouldNotBeNull();
            generationType.Name.ShouldEqual("G1");
            generationType.Template.ShouldEqual("g1_template");
            generationType.Description.ShouldEqual("the G1");
            generationType.Alterations.ShouldHaveTheSameElementsAs("g1a", "g1b");
        }

        [Test]
        public void can_load_tags_for_a_generation()
        {
            var generationType = theGraph.TemplateSetFor("G1");
            generationType.Tags.ShouldHaveTheSameElementsAs("x", "y", "z");
        }

        [Test]
        public void can_load_a_generation_type_without_template()
        {
            var g = theGraph.TemplateSetFor("G2");
            g.Template.ShouldBeEmpty();
            g.Alterations.ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void can_associate_options_with_a_generation()
        {
            var generationType = theGraph.TemplateSetFor("G1");
            generationType.Options.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Foo", "Bar");
        }

        // <option name='Foo' description='the Foo' alterations='e, f, g' />

        [Test]
        public void can_read_all_the_properties_of_an_option()
        {
            var generationType = theGraph.TemplateSetFor("G1");
            var fooOption = generationType.Options.First();

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
            var g = theGraph.TemplateSetFor("Complex");
            g.Selections.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Select1", "Select2");
        }

        [Test]
        public void can_read_the_description_for_a_selection()
        {
            var g = theGraph.TemplateSetFor("Complex");
            var select1 = g.Selections.First();

            select1.Description.ShouldEqual("the selection");

        }

        [Test]
        public void can_read_the_options_for_a_selection()
        {
            var g = theGraph.TemplateSetFor("Complex");
            var select1 = g.Selections.First();
            select1.Options.Select(x => x.Name).ShouldHaveTheSameElementsAs("Chiefs", "Broncos");
        }
    }
}