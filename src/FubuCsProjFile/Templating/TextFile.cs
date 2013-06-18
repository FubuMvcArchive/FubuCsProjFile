using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TextFile
    {
        public static readonly IFileSystem FileSystem = new FileSystem();

        private readonly string _path;

        public TextFile(string path)
        {
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }

        public string ReadAll()
        {
            return FileSystem.ReadStringFromFile(_path);
        }

        public IEnumerable<string> ReadLines()
        {
            return ReadAll().Trim().SplitOnNewLine();
        }
    }
}