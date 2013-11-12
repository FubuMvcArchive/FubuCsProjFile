using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class OptionSelectionTester
    {
        private OptionSelection theSelection;
        private TemplateChoices theChoices;
        private ProjectRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theSelection = new OptionSelection{Name = "select"};
            theSelection.Options.Add(new Option("a", "a1", "a2"));
            theSelection.Options.Add(new Option("b", "b1", "b2"));
            theSelection.Options.Add(new Option("c", "c1", "c2"));

            theChoices = new TemplateChoices();
            theRequest = new ProjectRequest("MyFoo", "baseline");
        }


        [Test]
        public void explicit_option_wins()
        {
            theChoices.Selections["select"] = "c";
            theChoices.Options = new string[]{"b"};

            theSelection.Configure(theChoices, theRequest);
            theRequest.Alterations.ShouldHaveTheSameElementsAs("c1", "c2");
        }

        [Test]
        public void no_known_options_use_the_first()
        {
            theSelection.Configure(theChoices, theRequest);
            theRequest.Alterations.ShouldHaveTheSameElementsAs("a1", "a2");
        }

        [Test]
        public void can_glean_a_selection_from_the_options()
        {
            theChoices.Options = new string[] { "b" };

            theSelection.Configure(theChoices, theRequest);
            theRequest.Alterations.ShouldHaveTheSameElementsAs("b1", "b2");
        }
    }
}