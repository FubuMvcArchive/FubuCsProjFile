using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;
using System.Linq;
using FubuCore.Util;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class Solution
    {
        private const string Global = "Global";
        private const string EndGlobal = "EndGlobal";
        public const string EndGlobalSection = "EndGlobalSection";
        public const string EndProjectSection = "EndProjectSection";
        private const string SolutionConfigurationPlatforms = "SolutionConfigurationPlatforms";
        private const string ProjectConfigurationPlatforms = "ProjectConfigurationPlatforms";

        public static readonly Guid SolutionFolderId = new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8");
        
        public static readonly string VS2010 = "VS2010";
        public static readonly string VS2012 = "VS2012";
        public static readonly string VS2013 = "VS2013";
        public static readonly string DefaultVersion = VS2010;

        private static readonly Cache<string, string[]> _versionLines = new Cache<string, string[]>();

        static Solution()
        {
            _versionLines[VS2010] = new[] { "Microsoft Visual Studio Solution File, Format Version 11.00", "# Visual Studio 2010" };
            _versionLines[VS2012] = new[] { "Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio 2012" };
            _versionLines[VS2013] = new[] { "Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio 2013", "VisualStudioVersion = 12.0.21005.1", "MinimumVisualStudioVersion = 10.0.40219.1" };
        }

        private readonly string _filename;
        private readonly IList<SolutionProject> _projects = new List<SolutionProject>(); 
        protected readonly IList<string> _header = new List<string>(); 
        /// <summary>
        /// Creates a new empty Solution file with the supplied name that
        /// will be written to the directory given upon calling Save()
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Solution CreateNew(string directory, string name)
        {
            var text = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (Solution), "Solution.txt")
                               .ReadAllText();

            var filename = directory.AppendPath(name);
            if (Path.GetExtension(filename) != ".sln")
            {
                filename = filename + ".sln";
            }

            return new Solution(filename, text)
            {
                Version = DefaultVersion
            };
        }

        /// <summary>
        /// Loads an existing solution from a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        private readonly IList<string> _globals = new List<string>(); 
        private readonly IList<GlobalSection> _sections = new List<GlobalSection>();

        /// <summary>
        /// Specify the VS.Net version.  At this time, the valid options are
        /// "VS2010" or "VS2012" or "VS2013"
        /// </summary>
        public string Version { get; set; }

        public string Filename
        {
            get { return _filename; }
        }

        public IList<GlobalSection> Sections
        {
            get { return _sections; }
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
            private ProjectSection _projectSection;
            private SolutionProject _solutionProject;

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
            private void lookForProjectSection(string text)
            {
                text = text.Trim();
                if (text.Trim().StartsWith("ProjectSection"))
                {
                    _projectSection = text.Trim().StartsWith("ProjectSection(ProjectDependencies)") ? new ProjectDependenciesSection(text) : new ProjectSection(text);
                    _solutionProject.ProjectSections.Add(_projectSection);
                    _read = readProjectSection;
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

            private void readProjectSection(string text)
            {
                if (text.Trim() == EndProjectSection)
                {
                    _read = readProject;
                }
                else
                {
                    _projectSection.Read(text);
                }
            }

            private void readProject(string text)
            {
                if (text.Trim().StartsWith("EndProject"))
                {
                    _read = normalRead;
                }
                else
                {
                    if (text.Trim().StartsWith("ProjectSection"))
                    {
                        lookForProjectSection(text);
                    }
                    else
                    {
                        _solutionProject.ReadLine(text);
                    }
                }
            }

            private void normalRead(string text)
            {
                if (text.StartsWith(Global))
                {
                    _read = lookForGlobalSection;
                }
                else if (text.StartsWith("ProjectSection"))
                {
                    _read = lookForProjectSection;
                }
                else if (IncludeAsProject(text))
                {
                    _solutionProject = new SolutionProject(text, _parent._filename.ParentDirectory());
                    _solutionProject.Solution = _parent;
                    _parent._projects.Add(_solutionProject);
                    _read = readProject;
                }
                else
                {
                    _parent._header.Add(text);
                    if (_parent.Version.IsEmpty())
                    {
                        foreach (var versionLine in _versionLines.ToDictionary())
                        {
                            if (text.Trim() == versionLine.Value[1])
                            {
                                _parent.Version = versionLine.Key;
                            }
                        }
                    }
                }
            }

            private static HashSet<string> ignoredLibraryTypes = new HashSet<string>
            {
                SolutionFolderId.ToString("B"),
                CsProjFile.VisualStudioSetupLibraryType.ToString("B"),
                CsProjFile.WebSiteLibraryType.ToString("B")
            }; 
            public static bool IncludeAsProject(string text)
            {
                return text.StartsWith("Project") && !ignoredLibraryTypes.Any(item => text.Contains(item, StringComparison.InvariantCultureIgnoreCase));                    
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

        /// <summary>
        /// Save the solution to the known file location
        /// </summary>
        public void Save()
        {
            Save(_filename);
        }

        /// <summary>
        /// Save the solution to a different file
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            calculateProjectConfigurationPlatforms();

            var writer = new StringWriter();

            this.EnsureHeaders();
            _header.Each(x => writer.WriteLine(x));

            _projects.Each(x => x.Write(writer));

            writer.WriteLine(Global);

            _sections.Each(x => x.Write(writer));

            writer.WriteLine(EndGlobal);

            new FileSystem().WriteStringToFile(filename, writer.ToString());

            _projects.Each(x => x.Project.Save());
        }

        private void EnsureHeaders()
        {
            if (_header.Count == 0)
            {
                _header.Add(string.Empty); // Visual studio project always start with a blank line.
                _versionLines.ToDictionary()[this.Version ?? DefaultVersion].Each(_header.Add);
            }
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

            if (section.Empty)
            {
                _sections.Remove(section);
            }
        }

        public IEnumerable<SolutionProject> Projects
        {
            get { return _projects; }
        }

        /// <summary>
        /// Attaches an existing project of this name to the solution or 
        /// creates a new project based on a Class Library and attaches
        /// to the solution
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public SolutionProject AddProject( string projectName)
        {
            var existing = FindProject(projectName);
            if (existing != null)
            {
                return existing;
            }

            var reference = SolutionProject.CreateNewAt(ParentDirectory, projectName);
            _projects.Add(reference);

            return reference;
        }

        /// <summary>
        /// Adds an existing project
        /// </summary>
        /// <param name="project"></param>
        public void AddProject(CsProjFile project)
        {
            var existing = FindProject(project.ProjectName);
            if (existing != null)
            {
                return;
            }
            
            var reference = new SolutionProject(project, this.ParentDirectory);
            this._projects.Add(reference);

        }

        /// <summary>
        /// Adds a new project based on the supplied template file
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        public SolutionProject AddProjectFromTemplate(string projectName, string templateFile)
        {
            var existing = FindProject(projectName);
            if (existing != null)
            {
                throw new ArgumentOutOfRangeException("projectName", "Project with this name ({0}) already exists in the solution".ToFormat(projectName));
            }


            var project = MSBuildProject.CreateFromFile(projectName, templateFile);
            var csProjFile = new CsProjFile(ParentDirectory.AppendPath(projectName, projectName + ".csproj"), project);
            csProjFile.ProjectGuid = Guid.NewGuid();

            var reference = new SolutionProject(csProjFile, ParentDirectory);
            _projects.Add(reference);

            return reference;
        }

        public void RemoveProject(CsProjFile project)
        {
            var existing = FindProject(project.ProjectName);
            if (existing == null)
            {
                return;
            }

            _projects.Remove(existing);
        }

        public string ParentDirectory
        {
            get { return _filename.ParentDirectory(); }
        }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(_filename); }
        }

        /// <summary>
        /// Access an attached project by name
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public SolutionProject FindProject(string projectName)
        {
            return _projects.FirstOrDefault(x => x.ProjectName == projectName);
        }
    }
}