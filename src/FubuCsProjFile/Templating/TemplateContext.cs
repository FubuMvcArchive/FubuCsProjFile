using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class TemplateContext
    {
        private readonly IFileSystem _fileSystem = new FileSystem();

        public static TemplateContext CreateClean(string directory)
        {
            var system = new FileSystem();
            system.CreateDirectory(directory);
            system.CleanDirectory(directory);

            return new TemplateContext(directory);
        }

        public TemplateContext(string rootDirectory)
        {
            Root = rootDirectory;
            SourceName = "src";
        }

        public string Root { get; set; }
        public string SourceName { get; set; }

        public string SourceDirectory
        {
            get { return Root.AppendPath(SourceName); }
        }

        public Solution Solution { get; set; }

        public IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        private readonly IList<ITemplateStep> _steps = new List<ITemplateStep>(); 


        public void Add(ITemplateStep step)
        {
            _steps.Add(step);
        }

        public void Execute()
        {
            _steps.Each(x => x.Alter(this));

            if (Solution != null)
            {
                Solution.Save();
            }
        }

        public void AlterFile(string relativeName, Action<List<string>> alter)
        {
            _fileSystem.AlterFlatFile(Root.AppendPath(relativeName), alter);
        }
    }
}