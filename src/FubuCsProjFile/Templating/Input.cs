using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class Input
    {
        public static readonly string File = "inputs.txt";

        public Input()
        {
        }

        public Input(string text)
        {
            string[] parts = text.ToDelimitedArray();
            if (parts.First().Contains("="))
            {
                string[] nameParts = parts.First().Split('=');
                Name = nameParts.First();
                Default = nameParts.Last();
            }
            else
            {
                Name = parts.First();
            }

            Description = parts.Last();
        }

        public string Name { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }

        public static IEnumerable<Input> ReadFrom(string directory)
        {
            var fileSystem = new FileSystem();
            string file = directory.AppendPath(File);
            if (!fileSystem.FileExists(file))
            {
                return Enumerable.Empty<Input>();
            }

            return ReadFromFile(file);
        }

        public static IEnumerable<Input> ReadFromFile(string file)
        {
            return new FileSystem().ReadStringFromFile(file).ReadLines().Where(x => x.IsNotEmpty())
                             .Select(x => new Input(x)).ToArray();
        }
    }
}