using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile
{
    public class ProjectReference
    {

        public static readonly string ProjectLineTemplate = "Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"";
        private readonly Guid _projectType;
        private readonly Guid _projectGuid;
        private readonly string _projectName;
        private readonly string _relativePath;

        private readonly IList<string> _directives = new List<string>();

        public ProjectReference(string text)
        {
            var parts = text.ToDelimitedArray('=');
            _projectType = Guid.Parse(parts.First().TextBetweenSquiggles());
            _projectGuid = Guid.Parse(parts.Last().TextBetweenSquiggles());

            var secondParts = parts.Last().ToDelimitedArray();
            _projectName = secondParts.First().TextBetweenQuotes();
            _relativePath = secondParts.ElementAt(1).TextBetweenQuotes().Replace("\\", "/"); // Windows is forgiving
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine(ProjectLineTemplate, _projectType.ToString().ToUpper(), _projectName, _relativePath.Replace('/', Path.DirectorySeparatorChar), _projectGuid.ToString().ToUpper());

            _directives.Each(x => writer.WriteLine(x));

            writer.WriteLine("EndProject");
        }

        public Guid ProjectGuid
        {
            get { return _projectGuid; }
        }

        public Guid ProjectType
        {
            get { return _projectType; }
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void ReadLine(string text)
        {
            _directives.Add(text);
        }
    }

    public static class CsProjFileExtensions
    {
        public static string TextBetweenSquiggles(this string text)
        {
            var start = text.IndexOf("{");
            var end = text.IndexOf("}");

            return text.Substring(start + 1, end - start - 1);
        }

        public static string TextBetweenQuotes(this string text)
        {
            return text.Trim().TrimStart('"').TrimEnd('"');
        }
    }
}