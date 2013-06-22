using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class AssemblyInfoAlteration : IProjectAlteration
    {
        public const string SourceFile = "assembly-info.txt";
        public const string AssemblyInfoPath = "Properties/AssemblyInfo.cs";
        private readonly IEnumerable<string> _additions;

        public AssemblyInfoAlteration(params string[] additions)
        {
            _additions = additions;
        }

        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            var codeFile = file.Find<CodeFile>(AssemblyInfoPath) ?? file.Add<CodeFile>(AssemblyInfoPath);

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
    }
}