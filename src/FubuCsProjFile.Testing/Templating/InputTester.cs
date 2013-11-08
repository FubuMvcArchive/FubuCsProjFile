using FubuCore;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class InputTester
    {
        [Test]
        public void parse_from_text_with_everything()
        {
            var input = new Input("Foo=%SHORTNAME%Foo,This is something");
            input.Name.ShouldEqual("Foo");
            input.Default.ShouldEqual("%SHORTNAME%Foo");
            input.Description.ShouldEqual("This is something");
        }

        [Test]
        public void parse_from_text_with_name_and_description()
        {
            var input = new Input("Foo,This is something");
            input.Name.ShouldEqual("Foo");
            input.Default.ShouldBeNull();
            input.Description.ShouldEqual("This is something");
        }

        [Test]
        public void parse_with_name_only()
        {
            var input = new Input("Foo");
            input.Name.ShouldEqual("Foo");
            input.Default.ShouldBeNull();
            input.Description.ShouldEqual("Foo");
        }

        [Test]
        public void read_inputs_returns_empty_with_no_file()
        {
            new FileSystem().DeleteFile("inputs.txt");

            Input.ReadFrom(".").Any().ShouldBeFalse();
        }

        [Test]
        public void read_inputs_with_some_values()
        {
            new FileSystem().WriteStringToFile("inputs.txt", @"Foo
Bar=%Foo%Bar,Some bar this is");

            var inputs = Input.ReadFrom(".");

            inputs.First().Name.ShouldEqual("Foo");
            var last = inputs.Last();

            last.Name.ShouldEqual("Bar");
            last.Default.ShouldEqual("%Foo%Bar");
            last.Description.ShouldEqual("Some bar this is");
        }
    }
}