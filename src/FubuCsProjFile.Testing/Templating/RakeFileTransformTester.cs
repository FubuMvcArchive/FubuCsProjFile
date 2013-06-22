using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FubuCsProjFile.Templating;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class RakeFileTransformTester
    {
        private TemplatePlan plan;
        private string rakefilePath;
        private string text;

        [SetUp]
        public void SetUp()
        {
            plan = TemplatePlan.CreateClean("rake");
            rakefilePath = "rake".AppendPath("rakefile");
        }

        public IEnumerable<string> theResultingContents()
        {
            new RakeFileTransform(text).Alter(plan);

            return new FileSystem().ReadStringFromFile(rakefilePath)
                                   .ReadLines();
        }

        [Test]
        public void writes_rake_file_if_none_exists()
        {
            text = @"
task :go do
  puts 'something'
end
";

            theResultingContents().ShouldContain("task :go do");
            theResultingContents().ShouldContain("  puts 'something'");
            theResultingContents().ShouldContain("end");
        }

        [Test]
        public void appends_to_rake_file_if_one_exists()
        {
            new FileSystem().WriteStringToFile(rakefilePath, "# I'm a comment");

            text = @"
task :go do
  puts 'something'
end
";

            theResultingContents().ShouldContain("# I'm a comment");
            theResultingContents().ShouldContain("task :go do");
            theResultingContents().ShouldContain("  puts 'something'");
            theResultingContents().ShouldContain("end");
        }

        [Test]
        public void does_some_substitution()
        {
            plan.Solution = Solution.CreateNew("rake".AppendPath("src"), "MySolution");

            text = "# %SOLUTION_NAME%";

            theResultingContents().ShouldContain("# MySolution");
        }

        [Test]
        public void will_not_double_up_content_for_a_single_line()
        {
            new FileSystem().WriteStringToFile(rakefilePath, "# something");

            text = "# something";

            theResultingContents().ShouldContain("# something");
            theResultingContents().Where(x => x.IsNotEmpty()).ShouldHaveCount(1);
        }

        [Test]
        public void will_not_double_up_content_for_multiple_lines()
        {
            new FileSystem().WriteStringToFile(rakefilePath, @"# I'm a comment

task :go do
  puts 'something'
end

");

            text = @"
task :go do
  puts 'something'
end
";

            var contents = theResultingContents().ToList();
            contents.Where(x => x == "task :go do").Count().ShouldEqual(1);
            contents.Where(x => x == "  puts 'something'").Count().ShouldEqual(1);
            contents.Where(x => x == "end").Count().ShouldEqual(1);
        }
    }
}