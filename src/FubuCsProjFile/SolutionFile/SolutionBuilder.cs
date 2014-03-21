using System.IO;
using System.Reflection;
using FubuCore;

namespace FubuCsProjFile.SolutionFile
{
    public static class SolutionBuilder
    {
        /// <summary>
        /// Creates a new empty Solution file with the supplied name that
        /// will be written to the directory given upon calling Save()
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ISolution CreateNew(string directory, string name)
        {
            var text = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (Solution), "Solution.txt")
                               .ReadAllText();

            var filename = directory.AppendPath(name);
            if (Path.GetExtension(filename) != ".sln")
            {
                filename = filename + ".sln";
            }

            var items = text.SplitOnNewLine();
            var solution = SolutionReader.Read(filename, items);
            solution.Version = SolutionFileVersioning.DefaultVersion;

            return solution;
        }
    }
}
