using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class TargetFrameworkVersionTester
    {
        [Test]
        public void can_parse_version_format_from_csproj_file()
        {
            var targetFramework = new TargetFrameworkVersion("v4.5.1");            
        }

        [Test]
        public void can_compare_two_instances_of_target_version_using_icomparable()
        {
            var higherFramework = new TargetFrameworkVersion("v4.5.1");            
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            higherFramework.ShouldBeGreaterThan(lowerFramework);
        }

        [Test]
        public void target_framework_greater_than_returns_true()
        {
            var higherFramework = new TargetFrameworkVersion("v4.5.1");            
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework > lowerFramework).ShouldBeTrue();
        }

        [Test]
        public void target_framework_greater_than_with_null_returns_false()
        {
            TargetFrameworkVersion higherFramework = null;
            TargetFrameworkVersion lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework > lowerFramework).ShouldBeFalse();
        }

        [Test]
        public void target_framework_greater_than_with_null_returns_true()
        {
            TargetFrameworkVersion higherFramework = new TargetFrameworkVersion("v4.5.1");
            TargetFrameworkVersion lowerFramework = null;

            (higherFramework > lowerFramework).ShouldBeTrue();
        }

        [Test]
        public void target_framework_greater_than_when_equal_returns_false()
        {
            var higherFramework = new TargetFrameworkVersion("v2.0");
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework > lowerFramework).ShouldBeFalse();
        }

        [Test]
        public void target_framework_less_than_returns_false()
        {
            var higherFramework = new TargetFrameworkVersion("v4.5.1");            
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework < lowerFramework).ShouldBeFalse();
        }

        [Test]
        public void target_framework_less_than_returns_true()
        {
            var higherFramework = new TargetFrameworkVersion("v4.5.1");            
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            (lowerFramework < higherFramework).ShouldBeTrue();
        }

        [Test]
        public void target_framework_less_than_with_null_returns_true()
        {
            TargetFrameworkVersion higherFramework = null;
            TargetFrameworkVersion lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework < lowerFramework).ShouldBeTrue();
        }

        [Test]
        public void target_framework_less_than_with_null_returns_false()
        {
            TargetFrameworkVersion higherFramework = new TargetFrameworkVersion("v4.5.1");
            TargetFrameworkVersion lowerFramework = null;

            (higherFramework < lowerFramework).ShouldBeFalse();
        }

        [Test]
        public void target_framework_less_than_when_equal_returns_false()
        {
            var higherFramework = new TargetFrameworkVersion("v2.0");
            var lowerFramework = new TargetFrameworkVersion("v2.0");

            (higherFramework < lowerFramework).ShouldBeFalse();
        }

        [Test]
        public void To_string_should_output_in_msbuild_format()
        {
            var targetFramework = new TargetFrameworkVersion("v4.5.1");
            targetFramework.ToString().ShouldEqual("v4.5.1");
        }

        [Test]
        public void Can_implicitly_convert_from_string_to_targetframework()
        {
            TargetFrameworkVersion framework = "v4.5.1";
            framework.ShouldNotBeNull();
        }

        [Test]
        public void Can_implicitly_convert_from_targetframework_to_string()
        {
            string framework = new TargetFrameworkVersion("v4.5.1");
            framework.ShouldEqual("v4.5.1");
        }
    }
}