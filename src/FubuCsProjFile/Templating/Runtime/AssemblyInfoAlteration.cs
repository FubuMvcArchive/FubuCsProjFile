using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.Templating.Runtime
{
    public class AssemblyInfoAlteration : IProjectAlteration
    {
        public const string SourceFile = "assembly-info.txt";
        public readonly string[] AssemblyInfoPath = new [] { "Properties", "AssemblyInfo.cs"};
        private readonly IEnumerable<string> _additions;

        public AssemblyInfoAlteration(params string[] additions)
        {
            _additions = additions;
        }

        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            var assemblyInfoPath = Path.Combine (AssemblyInfoPath);
            var codeFile = file.Find<CodeFile>(assemblyInfoPath) ?? file.Add<CodeFile>(assemblyInfoPath);

            var path = file.PathTo(codeFile);
            var parentDirectory = path.ParentDirectory();
            if (!Directory.Exists(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }

            new FileSystem().AlterFlatFile(path, contents => Alter(contents, plan));
        }

        public void Alter(List<string> contents, ProjectPlan plan)
        {
            _additions
                .Select(x => plan.ApplySubstitutions(x))
                .Where(x => !contents.Contains(x))
                .Each(contents.Add);
        }

        public override string ToString()
        {
            return string.Format("AssemblyInfo content:  {0}", _additions.Select(x => "'{0}'").Join("; "));
        }
    }
}