using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class SubstitutionsTester
    {
        [Test]
        public void set_value()
        {
            var substitutions = new Substitutions();
            substitutions.Set("key", "something");
            substitutions.Set("two", "twenty");

            substitutions.ValueFor("key").ShouldEqual("something");
            substitutions.ValueFor("two").ShouldEqual("twenty");

        }

        [Test]
        public void copy_to()
        {
            var substitutions = new Substitutions();
            substitutions.Set("key", "something");
            substitutions.Set("two", "twenty");

            var substitutions2 = new Substitutions();
            substitutions.CopyTo(substitutions2);

            substitutions2.ValueFor("key").ShouldEqual("something");
            substitutions2.ValueFor("two").ShouldEqual("twenty");
        }

        [Test]
        public void set_if_none_will_not_override_if_it_exists()
        {
            var substitutions = new Substitutions();
            substitutions.Set("key", "something");
            substitutions.SetIfNone("key", "different");

            substitutions.ValueFor("key").ShouldEqual("something");
        }

        [Test]
        public void set_if_none_will_write_if_no_previous_value()
        {
            var substitutions = new Substitutions();
            substitutions.SetIfNone("key", "different");

            substitutions.ValueFor("key").ShouldEqual("different");
        }

        [Test]
        public void set_overrides()
        {
            var substitutions = new Substitutions();
            substitutions.Set("key", "something");
            substitutions.Set("key", "different");

            substitutions.ValueFor("key").ShouldEqual("different");
        }

        [Test]
        public void apply_substitutions()
        {
            var substitutions = new Substitutions();
            substitutions.Set("%KEY%", "the key");
            substitutions.Set("%NAME%", "George Michael");

            var text = "%KEY% is %NAME%";
            substitutions.ApplySubstitutions(text)
                         .ShouldEqual("the key is George Michael");
        }

        [Test]
        public void write_and_read()
        {
            var substitutions = new Substitutions();
            substitutions.Set("key", "something");
            substitutions.Set("two", "twenty");

            substitutions.WriteTo("fubu.template.config");

            var substitutions2 = new Substitutions();
            substitutions2.ReadFrom("fubu.template.config");

            substitutions2.ValueFor("key").ShouldEqual("something");
            substitutions2.ValueFor("two").ShouldEqual("twenty");
        }

    }

    [TestFixture]
    public class when_reading_inputs
    {
        private Substitutions substitutions;

        [SetUp]
        public void SetUp()
        {
            substitutions = new Substitutions();
            substitutions.Set("%SHORT_NAME%", "BigProject");

            var inputs = new Input[]
            {
                new Input("%REGISTRY%=%SHORT_NAME%Registry"),
                new Input("Foo=Bar") ,
                new Input("%SHORT_NAME%=different") 
            };

            substitutions.ReadInputs(inputs);
        }

        [Test]
        public void does_not_overwrite()
        {
            substitutions.ValueFor("%SHORT_NAME%").ShouldEqual("BigProject");
        }

        [Test]
        public void imports_new_simple_input()
        {
            substitutions.ValueFor("Foo").ShouldEqual("Bar");
        }

        [Test]
        public void resolves_substitutions_in_input_value_defaults()
        {
            substitutions.ValueFor("%REGISTRY%").ShouldEqual("BigProjectRegistry");
        }
    }
}