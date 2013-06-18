using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class ProjectReference
    {
        public static ProjectReference CreateNewAt(string solutionDirectory, string projectName)
        {
            var csProjFile = CsProjFile.CreateAtSolutionDirectory(projectName, solutionDirectory);
            return new ProjectReference(csProjFile, solutionDirectory);
        }


        public static readonly string ProjectLineTemplate = "Project(\"{{{0}}}\") = \"{1}\", \"{2}\", \"{{{3}}}\"";
        private readonly Guid _projectType;
        private readonly Guid _projectGuid;
        private readonly string _projectName;
        private readonly string _relativePath;

        private readonly IList<string> _directives = new List<string>();
        private readonly Lazy<CsProjFile> _project;

        public ProjectReference(CsProjFile csProjFile, string solutionDirectory)
        {
            _project = new Lazy<CsProjFile>(() => csProjFile);
            _projectName = csProjFile.ProjectName;
            _relativePath = csProjFile.FileName.PathRelativeTo(solutionDirectory);
            _projectType = csProjFile.ProjectTypes().FirstOrDefault();
            _projectGuid = csProjFile.ProjectGuid;
        }

        public ProjectReference(string text, string solutionDirectory)
        {
            var parts = text.ToDelimitedArray('=');
            _projectType = Guid.Parse(parts.First().TextBetweenSquiggles());
            _projectGuid = Guid.Parse(parts.Last().TextBetweenSquiggles());

            var secondParts = parts.Last().ToDelimitedArray();
            _projectName = secondParts.First().TextBetweenQuotes();
            _relativePath = secondParts.ElementAt(1).TextBetweenQuotes().Replace("\\", "/"); // Windows is forgiving


            _project = new Lazy<CsProjFile>(() => {
                var filename = solutionDirectory.AppendPath(_relativePath);

                if (File.Exists(filename))
                {
                    return CsProjFile.LoadFrom(filename);
                }
                
                var project = CsProjFile.CreateAtLocation(filename, _projectName);
                project.ProjectGuid = _projectGuid;

                return project;
            });
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

        public CsProjFile Project
        {
            get { return _project.Value; }
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