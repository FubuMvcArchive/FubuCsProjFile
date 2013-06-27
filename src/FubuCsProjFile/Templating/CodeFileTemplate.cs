using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;
using System.Linq;

namespace FubuCsProjFile.Templating
{
    public class CodeFileTemplate : IProjectAlteration
    {
        public const string CLASS = "%CLASS%";

        private readonly string _relativePath;
        private readonly string _rawText;

        public static CodeFileTemplate Class(string relativePath)
        {
            var @class = Path.GetFileNameWithoutExtension(relativePath);

            var rawText = Assembly.GetExecutingAssembly()
                                  .GetManifestResourceStream(typeof (CodeFileTemplate), "Class.txt")
                                  .ReadAllText()
                                  .Replace(CLASS, @class);

            if (Path.GetExtension(relativePath) != ".cs")
            {
                relativePath = relativePath + ".cs";
            }

            return new CodeFileTemplate(relativePath, rawText);
        }

        public CodeFileTemplate(string relativePath, string rawText)
        {
            if (Path.GetExtension(relativePath) != ".cs")
            {
                throw new ArgumentOutOfRangeException("relativePath", "Relative Path must have the .cs extension");
            }

            _relativePath = relativePath.Replace('\\', '/');

            _rawText = rawText;
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public string RawText
        {
            get { return _rawText; }
        }



        public void Alter(CsProjFile file, ProjectPlan plan)
        {
            var includePath = plan.ApplySubstitutions(_relativePath);
            var filename = file.FileName.ParentDirectory().AppendPath(includePath);
            if (!filename.EndsWith(".cs"))
            {
                filename = filename + ".cs";
            }

            var text = plan.ApplySubstitutions(_rawText, _relativePath);

            new FileSystem().WriteStringToFile(filename, text);

            file.Add<CodeFile>(includePath);
        }

        public override string ToString()
        {
            return string.Format("Write and attach code file: {0}", _relativePath);
        }
    }
}