using System.Collections.Generic;
using System.Linq;
using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.ProjectFiles
{
    public class ProjectLoader
    {
        private static readonly IList<IProjectLoader> Loaders = new List<IProjectLoader>
        {
            new CsProjLoader()
        }; 

        public static IProjectFile Load(string filename)
        {
            var loader = Loaders.FirstOrDefault(x => x.Matches(filename));

            if (loader == null) return null;
                
            return loader.Load(filename);
        }
    }
}