using System;
using System.Collections.Generic;
using FubuCsProjFile.Templating.Graph;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class TemplateGraph_building_project_request_from_TemplateChoices
    {
        [Test]
        public void throw_if_generation_name_is_empty()
        {
            Exception<Exception>.ShouldBeThrownBy(() => {
                new TemplateGraph().Configure(new TemplateChoices());
            }).Message.ShouldContain("SetName is required");
        }

        [Test]
        public void throw_if_project_name_cannot_be_found()
        {
            Exception<Exception>.ShouldBeThrownBy(() =>
            {
                new TemplateGraph().Configure(new TemplateChoices { SetName = "Foo" });
            }).Message.ShouldContain("ProjectName is required");
        }

        [Test]
        public void throw_if_named_generation_cannot_be_found()
        {
            Exception<Exception>.ShouldBeThrownBy(() =>
            {
                new TemplateGraph().Configure(new TemplateChoices{SetName = "Foo", ProjectName = "MyFoo"});
            }).Message.ShouldContain("TemplateSet 'Foo' is unknown");
        }

        [Test]
        public void throw_if_found_generation_set_does_not_match_the_tag()
        {
            var graph = new TemplateGraph();
            graph.Add(new TemplateSet
            {
                Name = "Foo",
                Tags = new List<string>{"A", "B"}
            });

            Exception<Exception>.ShouldBeThrownBy(() => {
                graph.Configure(new TemplateChoices {SetName = "Foo", Tag = "C", ProjectName = "MyFoo"});
            }).Message.ShouldContain("TemplateSet 'Foo' is not tagged as a valid 'C'");
        }

        [Test]
        public void build_request_with_matching_template_and_options()
        {
            var graph = new TemplateGraph();
            graph.Add(new TemplateSet
            {
                Name = "Foo",
                Tags = new List<string> { "A", "B" },
                Template = "FooProj",
                Alterations = new List<string>{"Foo1", "Foo2"}
            });

            var choices = new TemplateChoices {SetName = "Foo", Tag = "A", ProjectName = "MyFoo"};

            var request = graph.Configure(choices);

            request.Name.ShouldEqual(choices.ProjectName);
            request.Template.ShouldEqual("FooProj");
            request.Alterations.ShouldHaveTheSameElementsAs("Foo1", "Foo2");
        }
    }
}