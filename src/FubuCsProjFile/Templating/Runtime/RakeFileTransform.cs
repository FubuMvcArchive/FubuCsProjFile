using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCsProjFile.Templating.Planning;

namespace FubuCsProjFile.Templating.Runtime
{
    public class RakeFileTransform : ITemplateStep
    {
        private readonly string _text;
        public static readonly string TargetFile = "rakefile";
        public static readonly string SourceFile = "rake.txt";

        public RakeFileTransform(string text)
        {
            _text = text;
        }

        public static string FindFile(string directory)
        {
            if (File.Exists(directory.AppendPath("rakefile.rb")))
            {
                return directory.AppendPath("rakefile.rb").ToFullPath();
            }

            return directory.AppendPath(TargetFile).ToFullPath();
        }

        public void Alter(TemplatePlan plan)
        {
            var lines = plan.ApplySubstitutions(_text).SplitOnNewLine();

            var rakeFile = FindFile(plan.Root);
            var fileSystem = new FileSystem();


            var list =
                fileSystem.FileExists(rakeFile)
                    ? fileSystem.ReadStringFromFile(rakeFile).ReadLines().ToList()
                    : new List<string>();

            if (list.ContainsSequence(lines))
            {
                return;
            }

            list.Add(string.Empty);
            list.AddRange(lines);

            fileSystem.WriteStringToFile(rakeFile, list.Join(Environment.NewLine));
        }

        public override string ToString()
        {
            return "Add content to the rakefile:";
        }
    }
}