using FubuCore;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class GemReferenceTester
    {
        [Test]
        public void write_gem_file_when_none_exists()
        {
            var context = TemplatePlan.CreateClean("gems");
            var gem = new GemReference("rake", ">=10.0.3");

            gem.Alter(context);

            var gemFile = "gems".AppendPath("Gemfile");
            var contents = new FileSystem().ReadStringFromFile(gemFile)
                                           .SplitOnNewLine();

            contents.ShouldContain("source 'http://rubygems.org'");
            contents.ShouldContain("gem \"rake\", \">=10.0.3\"");
        }

        [Test]
        public void do_not_add_gem_when_there_is_already_a_reference()
        {
            var gemFile = "gems".AppendPath("Gemfile");


            var context = TemplatePlan.CreateClean("gems");

            new FileSystem().WriteStringToFile(gemFile, @"source 'http://rubygems.org'

gem ~rake~, ~>=10.0.3~
".Replace("~", "\""));

            var gem = new GemReference("rake", ">=10.0.4");

            gem.Alter(context);

            var contents = new FileSystem().ReadStringFromFile(gemFile)
                                           .SplitOnNewLine();

            contents.ShouldContain("source 'http://rubygems.org'");
            contents.ShouldContain("gem \"rake\", \">=10.0.3\""); // didn't change
            contents.ShouldNotContain("gem \"rake\", \">=10.0.4\""); // didn't change
        }

        [Test]
        public void do_add_gem_to_existing()
        {
            var gemFile = "gems".AppendPath("Gemfile");
            new FileSystem().WriteStringToFile(gemFile, @"source 'http://rubygems.org'

gem ~rake~, ~>=10.0.3~
".Replace("~", "\""));

            var context = TemplatePlan.CreateClean("gems");
            var gem = new GemReference("fuburake", "~>0.5");

            gem.Alter(context);

            var contents = new FileSystem().ReadStringFromFile(gemFile)
                                           .SplitOnNewLine();

            contents.ShouldContain("source 'http://rubygems.org'");
            contents.ShouldContain("gem \"fuburake\", \"~>0.5\""); // didn't change
        }
    }
}