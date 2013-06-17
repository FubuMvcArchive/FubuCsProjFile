
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuCsProjFile.MSBuild;
using System.Linq;

namespace FubuCsProjFile
{
    public class CsProjFile
    {
        private const string PROJECTGUID = "ProjectGuid";
        private readonly string _fileName;
        private readonly MSBuildProject _project;
        public static readonly Guid ClassLibraryType = Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");

        public CsProjFile(string fileName) : this(fileName, MSBuildProject.LoadFrom(fileName))
        {
        }

        private CsProjFile(string fileName, MSBuildProject project)
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

        public void Add<T>(T item) where T : ProjectItem
        {
            var group = _project.FindGroup(item.Matches) ?? _project.FindGroup(x => x.Name == item.Name) ?? _project.AddNewItemGroup();
            item.Configure(group);
        }

        public IEnumerable<T> All<T>() where T : ProjectItem, new()
        {
            var name = new T().Name;

            return _project.GetAllItems(name).OrderBy(x => x.Include)
                           .Select(item => {
                               var projectItem = new T();
                               projectItem.Read(item);

                               return projectItem;
                           });
        } 

        public T Add<T>(string include) where T : ProjectItem, new()
        {
            var item = new T {Include = include};
            Add(item);

            return item;
        }

        public static CsProjFile CreateAtSolutionDirectory(string assemblyName, string directory)
        {
            var fileName = directory.AppendPath(assemblyName).AppendPath(assemblyName) + ".csproj";
            var project = MSBuildProject.Create(assemblyName);
            var group = project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name == PROJECTGUID)) ?? project.PropertyGroups.FirstOrDefault() ?? project.AddNewPropertyGroup(true);

            group.SetPropertyValue(PROJECTGUID, Guid.NewGuid().ToString().ToUpper(), true);

            return new CsProjFile(fileName, project);
        }

        public static CsProjFile LoadFrom(string filename)
        {
            var project = MSBuildProject.LoadFrom(filename);
            return new CsProjFile(filename, project);
        }

        public static CsProjFile CreateAtLocation(string filename, string assemblyName)
        {
            var project = MSBuildProject.Create(assemblyName);
            return new CsProjFile(filename, project);
        }

        public string ProjectName
        {
            get { return Path.GetFileNameWithoutExtension(_fileName); }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public void Save()
        {
            _project.Save(_fileName);
        }

        public IEnumerable<Guid> ProjectTypes()
        {
            var raws =
                _project.PropertyGroups.Select(x => x.GetPropertyValue("ProjectTypeGuids")).Where(x => x.IsNotEmpty());

            if (raws.Any())
            {
                foreach (var raw in raws)
                {
                    foreach ( var guid in raw.Split(';'))
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
    }
}