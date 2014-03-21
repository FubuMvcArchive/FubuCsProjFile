using System.Collections;
using System.Collections.Generic;
using FubuCsProjFile.SolutionFile;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.SolutionFile
{
    [TestFixture]
    public class BuildConfigurationTester
    {
        [Test]
        public void create_from_text()
        {
            var config = new BuildConfiguration("\t\tDebug|Mixed PlatformsKEY = Debug|Mixed Platforms");
            config.Key.ShouldEqual("Debug|Mixed PlatformsKEY");
            config.Value.ShouldEqual("Debug|Mixed Platforms");
        }

        [TestCaseSource("equals_source")]
        public bool equals(BuildConfiguration config1, BuildConfiguration config2)
        {
            return config1.Equals(config2);
        }

        [TestCaseSource("equals_source_object")]
        public bool equals_obj(BuildConfiguration config, object obj)
        {
            return config.Equals(obj);
        }

        public IList<BuildConfiguration> configurations = new List<BuildConfiguration>
        {
            new BuildConfiguration {Key = "key1", Value = "value1"},
            new BuildConfiguration {Key = "key1", Value = "value1"},
            new BuildConfiguration {Key = "key1", Value = "value2"},
            new BuildConfiguration {Key = "key2", Value = "value1"},
            new BuildConfiguration {Key = "key2", Value = "value2"}
        };

        public IEnumerable<TestCaseData> equals_source()
        {
            yield return new TestCaseData(configurations[0], configurations[0]).Returns(true).SetName("Reference equality");
            yield return new TestCaseData(configurations[0], configurations[1]).Returns(true).SetName("Key and Value equality");
            yield return new TestCaseData(configurations[0], configurations[2]).Returns(false).SetName("Key equal, Value different");
            yield return new TestCaseData(configurations[0], configurations[3]).Returns(false).SetName("Key different, Value equal");
            yield return new TestCaseData(configurations[0], configurations[4]).Returns(false).SetName("Key different, Value different");
        }

        public IEnumerable<TestCaseData> equals_source_object()
        {
            return equals_source().UnionWith(new TestCaseData(configurations[0], new object()).Returns(false).SetName("Different object types"));
        }
    }
}