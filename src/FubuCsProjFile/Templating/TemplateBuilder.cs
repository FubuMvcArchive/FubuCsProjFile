using System;
using System.IO;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TemplateBuilder
    {
        private readonly string _directory;

        public TemplateBuilder(string directory)
        {
            _directory = directory;

            TemplateLibrary.FileSystem.CreateDirectory(directory);
        }

        public void WriteContents(string relativePath, string contents)
        {
            TemplateLibrary.FileSystem.WriteStringToFile(_directory.AppendPath(relativePath), contents);
        }

        public void WriteContents(string file, Action<StringWriter> action)
        {
            var writer = new StringWriter();
            action(writer);

            WriteContents(file, writer.ToString());
        }

        public void WriteDescription(string text)
        {
            WriteContents(TemplateLibrary.DescriptionFile, text);
        }
    }
}