using FubuCore;
using FubuCsProjFile.SolutionFile;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class Solution_can_read_and_write_with_versions_Tester
    {
        [Test]
        public void write_and_read_as_VS2010()
        {
            new FileSystem().DeleteDirectory("VS2010");

            var solution = SolutionBuilder.CreateNew("VS2010", "VS2010");
            solution.Version = SolutionFileVersioning.VS2010;
            solution.Save();

            var solution2 = SolutionReader.LoadFrom(solution.Filename);
            solution2.Version.ShouldEqual(SolutionFileVersioning.VS2010);

            //Process.Start(solution.Filename.ToFullPath());
        }

        [Test]
        public void write_and_read_as_VS2012()
        {
            new FileSystem().DeleteDirectory("VS2012");

            var solution = SolutionBuilder.CreateNew("VS2012", "VS2012");
            solution.Version = SolutionFileVersioning.VS2012;
            solution.Save();

            var solution2 = SolutionReader.LoadFrom(solution.Filename);
            solution2.Version.ShouldEqual(SolutionFileVersioning.VS2012);

            //Process.Start(solution.Filename.ToFullPath());
        }

        [Test]
        public void write_and_read_as_VS2013()
        {
            new FileSystem().DeleteDirectory("VS2013");

            var solution = SolutionBuilder.CreateNew("VS2013", "VS2013");
            solution.Version = SolutionFileVersioning.VS2013;
            solution.Save();

            var solution2 = SolutionReader.LoadFrom(solution.Filename);
            solution2.Version.ShouldEqual(SolutionFileVersioning.VS2013);

            //Process.Start(solution.Filename.ToFullPath());
        }

        [Test]
        public void setting_the_version()
        {
            // SAMPLE: setting-version
            var solution = SolutionBuilder.CreateNew("MySolution", "MySolution");
            solution.Version = SolutionFileVersioning.VS2010;
            solution.Version = SolutionFileVersioning.VS2012;
            solution.Version = SolutionFileVersioning.VS2013;
            // ENDSAMPLE
        }
    }
}