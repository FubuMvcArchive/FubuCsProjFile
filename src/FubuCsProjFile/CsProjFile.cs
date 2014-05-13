using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Xml;
using FubuCore;
using FubuCsProjFile.MSBuild;
using System.Linq;

namespace FubuCsProjFile
{
    public static class DotNetVersion
    {
        public static readonly string V40 = "v4.0";
        public static readonly string V45 = "v4.5";
    }

    public class CsProjFile
    {
        /*
             <RootNamespace>MyProject</RootNamespace>
    <AssemblyName>MyProject</AssemblyName>
         */
        private const string PROJECTGUID = "ProjectGuid";
        private const string ROOT_NAMESPACE = "RootNamespace";
        private const string ASSEMBLY_NAME = "AssemblyName";

        private readonly string _fileName;
        private readonly MSBuildProject _project;
        private readonly Dictionary<string, ProjectItem> _projectItemCache = new Dictionary<string, ProjectItem>();
        public static readonly Guid ClassLibraryType = Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        public static readonly Guid WebSiteLibraryType = new Guid("E24C65DC-7377-472B-9ABA-BC803B73C61A");
        public static readonly Guid VisualStudioSetupLibraryType = new Guid("54435603-DBB4-11D2-8724-00A0C9A8B90C");
        public CsProjFile(string fileName) : this(fileName, MSBuildProject.LoadFrom(fileName))
        {
        }

        public CsProjFile(string fileName, MSBuildProject project)
        {
            _fileName = fileName;
            _project = project;
        }

        public Guid ProjectGuid
        {
            get
            {
                var raw = _project.PropertyGroups.Select(x => x.GetPropertyValue(PROJECTGUID))
                                  .FirstOrDefault(x => x.IsNotEmpty());

                return raw.IsEmpty() ? Guid.Empty : Guid.Parse(raw.TrimStart('{').TrimEnd('}'));

            }
            internal set
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == PROJECTGUID))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);

                group.SetPropertyValue(PROJECTGUID, value.ToString().ToUpper(), true);
            }
        }

        public string AssemblyName
        {
            get
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == ASSEMBLY_NAME))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);

                return group.GetPropertyValue(ASSEMBLY_NAME);
            }
            set
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == ASSEMBLY_NAME))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);
                group.SetPropertyValue(ASSEMBLY_NAME, value, true);
            }
        }

        public string RootNamespace
        {
            get
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == ROOT_NAMESPACE))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);

                return group.GetPropertyValue(ROOT_NAMESPACE);
            }
            set
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == ROOT_NAMESPACE))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);
                group.SetPropertyValue(ROOT_NAMESPACE, value, true);
            }
        }

        public void Add<T>(T item) where T : ProjectItem
        {
            var group = _project.FindGroup(item.Matches) ??
                        _project.FindGroup(x => x.Name == item.Name) ?? _project.AddNewItemGroup();
            item.Configure(group);
        }

        public T Add<T>(string include) where T : ProjectItem, new()
        {
            var item = new T {Include = include};

            _projectItemCache.Remove(item.Include);            
            _projectItemCache.Add(include, item);
            Add(item);

            return item;
        }
        
        public IEnumerable<T> All<T>() where T : ProjectItem, new()
        {
            var name = new T().Name;

            return _project.GetAllItems(name).OrderBy(x => x.Include)
                           .Select(item =>
                           {
                               T projectItem;
                               if (_projectItemCache.ContainsKey(item.Include))
                               {
                                   projectItem = (T) _projectItemCache[item.Include];
                               }
                               else
                               {
                                   projectItem = new T();
                                   projectItem.Read(item);                               
                                   _projectItemCache.Add(item.Include, projectItem);
                               }

                               return projectItem;
                           });
        }

        /// <summary>
        /// Creates a new CsProjFile in a folder relative to the solution folder
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static CsProjFile CreateAtSolutionDirectory(string assemblyName, string directory)
        {
            var fileName = directory.AppendPath(assemblyName).AppendPath(assemblyName) + ".csproj";
            var project = MSBuildProject.Create(assemblyName);
            return CreateCore(project, fileName);
        }

        /// <summary>
        /// Creates a new class library project at the given filename
        /// and assembly name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static CsProjFile CreateAtLocation(string fileName, string assemblyName)
        {
            return CreateCore(MSBuildProject.Create(assemblyName), fileName);
        }

        private static CsProjFile CreateCore(MSBuildProject project, string fileName)
        {
            var group = project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == PROJECTGUID)) ??
                        project.PropertyGroups.FirstOrDefault() ?? project.AddNewPropertyGroup(true);

            @group.SetPropertyValue(PROJECTGUID, Guid.NewGuid().ToString().ToUpper(), true);

            var file = new CsProjFile(fileName, project);
            file.AssemblyName = file.RootNamespace = file.ProjectName;
            return file;
        }

        /// <summary>
        /// Load an existing CsProjFile from the filename given
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static CsProjFile LoadFrom(string filename)
        {
            var project = MSBuildProject.LoadFrom(filename);
            return new CsProjFile(filename, project);
        }

        public string ProjectName
        {
            get { return Path.GetFileNameWithoutExtension(_fileName); }
        }

        public MSBuildProject BuildProject
        {
            get { return this._project; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public string ProjectDirectory
        {
            get { return _fileName.ParentDirectory(); }
        }

        public FrameworkName FrameworkName
        {
            get { return _project.FrameworkName; }
        }

        public string DotNetVersion
        {
            get
            {
                return _project.PropertyGroups.Select(x => x.GetPropertyValue("TargetFrameworkVersion"))
                                  .FirstOrDefault(x => x.IsNotEmpty());

            }
            set
            {
                var group = _project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == "TargetFrameworkVersion"))
                            ?? _project.PropertyGroups.FirstOrDefault() ?? _project.AddNewPropertyGroup(true);

                group.SetPropertyValue("TargetFrameworkVersion", value, true);
            }
        }

        public SourceControlInformation SourceControlInformation { get; set; }

        private AssemblyInfo assemblyInfo;
        public AssemblyInfo AssemblyInfo
        {
            get
            {
                if (assemblyInfo == null)
                {
                    var codeFile = this.All<CodeFile>().FirstOrDefault(item => item.Include.EndsWith("AssemblyInfo.cs"));
                    if (codeFile != null)
                    {
                        assemblyInfo = new AssemblyInfo(codeFile, this);
                        this._projectItemCache.Add("AssemblyInfo+" + codeFile.Include, assemblyInfo);
                    }
                }

                return assemblyInfo;
            } 
        }

        public TargetFrameworkVersion TargetFrameworkVersion
        {
            get
            {
                return this.BuildProject.GetGlobalPropertyGroup().GetPropertyValue("TargetFrameworkVersion");
            }
            set
            {
                this.BuildProject.GetGlobalPropertyGroup().SetPropertyValue("TargetFrameworkVersion", value, false);
            }
        }


        public string Platform
        {
            get
            {
                return this.BuildProject.GetGlobalPropertyGroup().GetPropertyValue("Platform");
            }
            set
            {
                this.BuildProject.GetGlobalPropertyGroup().SetPropertyValue("Platform", value, false);
            }
        }

        public void Save()
        {
            this.Save(_fileName);
        }

        public void Save(string file)
        {
            foreach (var item in this._projectItemCache)
            {
                item.Value.Save();
            }

            _project.Save(file);
        }

        public IEnumerable<Guid> ProjectTypes()
        {
            var raws =
                _project.PropertyGroups.Select(x => x.GetPropertyValue("ProjectTypeGuids")).Where(x => x.IsNotEmpty());

            if (raws.Any())
            {
                foreach (var raw in raws)
                {
                    foreach (var guid in raw.Split(';'))
                    {
                        yield return Guid.Parse(guid.TrimStart('{').TrimEnd('}'));
                    }
                }
            }
            else
            {
                yield return ClassLibraryType; // Class library
            }
        }


        public void CopyFileTo(string source, string relativePath)
        {
            var target = _fileName.ParentDirectory().AppendPath(relativePath);
            new FileSystem().Copy(source, target);
        }

        public T Find<T>(string include) where T : ProjectItem, new()
        {
            return All<T>().FirstOrDefault(x => x.Include == include);
        }

        public string PathTo(CodeFile codeFile)
        {
            var path = codeFile.Include;
            if (FubuCore.Platform.IsUnix ()) {
                path = path.Replace ('\\', Path.DirectorySeparatorChar);
            }
            return _fileName.ParentDirectory().AppendPath(path);
        }


        public void Remove<T>(string include) where T : ProjectItem, new()
        {
            var name = new T().Name;
            
            _projectItemCache.Remove(include);
            
            var element = _project.GetAllItems(name).FirstOrDefault(x => x.Include == include);
            if (element != null)
            {
                element.Remove();
            }
        }

        public void Remove<T>(T item) where T : ProjectItem, new()
        {
            _projectItemCache.Remove(item.Include);
                
            var element = _project.GetAllItems(item.Name).FirstOrDefault(x => x.Include == item.Include);
            if (element != null)
            {
                element.Remove();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.FileName);
        }
    }
}


