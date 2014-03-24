using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FubuCsProjFile
{
    public class ProjectDependenciesSection : ProjectSection
    {
        public ProjectDependenciesSection() : this("\tProjectSection(ProjectDependencies) = postProject")
        {            
        }

        public ProjectDependenciesSection(string declaration) : base(declaration)
        {
        }

        public ReadOnlyCollection<Guid> Dependencies
        {
            get
            {
                return this.Properties.Select(x => new Guid(x.Split('=')[0])).ToList().AsReadOnly();                
            }
        }

        public void Add(Guid projectGuid)
        {
            var itemAdding = projectGuid.ToString("B").ToUpper();
                
            if (_properties.Any(item => item.ToUpper().StartsWith(itemAdding)))
            {
                return;
            }

            this._properties.Add(string.Format("{0} = {0}", itemAdding));
        }

        public void Remove(Guid projectGuid)
        {
            for (int i = this.Properties.Count -1; i <= 0; i++)
            {
                var line = this._properties[i];
                if (line.ToUpper().StartsWith(projectGuid.ToString("B").ToUpper()))
                {
                    this._properties.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            this._properties.Clear();
        }
    }
}