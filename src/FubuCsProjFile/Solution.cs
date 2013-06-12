using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;
using System.Linq;
using FubuCore.Util;

namespace FubuCsProjFile
{
    public class Solution
    {
        private const string Global = "Global";
        private const string EndGlobal = "EndGlobal";
        public const string EndGlobalSection = "EndGlobalSection";
        private const string SolutionConfigurationPlatforms = "SolutionConfigurationPlatforms";
        private const string ProjectConfigurationPlatforms = "ProjectConfigurationPlatforms";

        private readonly string _filename;
        private readonly IList<ProjectReference> _projects = new List<ProjectReference>(); 

        public static Solution CreateNew(string directory, string name)
        {
            var text = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (Solution), "Solution.txt")
                               .ReadAllText();

            var filename = directory.AppendPath(name);
            if (!Path.HasExtension(filename))
            {
                filename = filename + ".sln";
            }

            return new Solution(filename, text);
        }

        public static Solution LoadFrom(string filename)
        {
            var text = new FileSystem().ReadStringFromFile(filename);
            return new Solution(filename, text);
        }

        private Solution(string filename, string text)
        {
            _filename = filename;
            var items = text.SplitOnNewLine();
            var reader = new SolutionReader(this);
            items.Each(reader.Read);
        }

        private readonly IList<string> _preamble = new List<string>();
        private readonly IList<string> _globals = new List<string>(); 
        private readonly IList<GlobalSection> _sections = new List<GlobalSection>(); 
        

        public string Filename
        {
            get { return _filename; }
        }

        public IList<GlobalSection> Sections
        {
            get { return _sections; }
        }

        public IEnumerable<string> Preamble
        {
            get { return _preamble; }
        }

        public IEnumerable<string> Globals
        {
            get { return _globals; }
        }

        public IEnumerable<BuildConfiguration> Configurations()
        {
            var section = FindSection(SolutionConfigurationPlatforms);
            return section == null
                       ? Enumerable.Empty<BuildConfiguration>()
                       : section.Properties.Select(x => new BuildConfiguration(x));
        }

        public class SolutionReader
        {
            private readonly Solution _parent;
            private Action<string> _read;
            private GlobalSection _section;
            private ProjectReference _project;

            public SolutionReader(Solution parent)
            {
                _parent = parent;

                _read = normalRead;
            }

            private void lookForGlobalSection(string text)
            {
                text = text.Trim();
                if (text.Trim().StartsWith("GlobalSection"))
                {
                    _section = new GlobalSection(text);
                    _parent._sections.Add(_section);
                    _read = readSection;
                }
            }

            private void readSection(string text)
            {
                if (text.Trim() == EndGlobalSection)
                {
                    _read = lookForGlobalSection;
                }
                else
                {
                    _section.Read(text);
                }
            }

            private void readProject(string text)
            {
                if (text.StartsWith("EndProject"))
                {
                    _read = normalRead;
                }
                else
                {
                    _project.ReadLine(text);
                }
            }

            private void normalRead(string text)
            {
                if (text.StartsWith(Global))
                {
                    _read = lookForGlobalSection;
                }
                else if (text.StartsWith("Project"))
                {
                    _project = new ProjectReference(text);
                    _parent._projects.Add(_project);
                    _read = readProject;
                }
                else
                {
                    _parent._preamble.Add(text);
                }
            }

            public void Read(string text)
            {
                _read(text);
            }


        }

        public GlobalSection FindSection(string name)
        {
            return _sections.FirstOrDefault(x => x.SectionName == name);
        }

        public void Save()
        {
            Save(_filename);
        }

        public void Save(string filename)
        {
            calculateProjectConfigurationPlatforms();

            var writer = new StringWriter();

            _preamble.Each(x => writer.WriteLine(x));

            _projects.Each(x => x.Write(writer));

            writer.WriteLine(Global);

            _sections.Each(x => x.Write(writer));

            writer.WriteLine(EndGlobal);


            new FileSystem().WriteStringToFile(filename, writer.ToString().TrimEnd());
        }

        private void calculateProjectConfigurationPlatforms()
        {
            var section = FindSection(ProjectConfigurationPlatforms);
            if (section == null)
            {
                section = new GlobalSection("GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                _sections.Add(section);
            }

            section.Properties.Clear();
            var configurations = Configurations().ToArray();

            _projects.Where(x => x.ProjectName != "Solution Items").Each(proj => {
                configurations.Each(config => config.WriteProjectConfiguration(proj, section));
            });


        }

        public IEnumerable<ProjectReference> Projects
        {
            get { return _projects; }
        }
    }
}