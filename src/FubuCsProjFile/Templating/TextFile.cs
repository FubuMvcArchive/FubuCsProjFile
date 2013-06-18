using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TextFile
    {
        public static readonly IFileSystem FileSystem = new FileSystem();

        private readonly string _path;
        private readonly string _relativePath;

        public TextFile(string path, string relativePath)
        {
            _path = path;
            _relativePath = relativePath.Replace('\\', '/');
        }

        public string RelativePath
        {
            get { return _relativePath; }
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