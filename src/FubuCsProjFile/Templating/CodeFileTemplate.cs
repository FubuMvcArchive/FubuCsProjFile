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
        public const string ASSEMBLY_NAME = "%ASSEMBLYNAME%";
        public const string NAMESPACE = "%NAMESPACE%";
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

        public static string GetNamespace(string relativePath, string projectName)
        {
            return relativePath
                .Split('/')
                .Reverse()
                .Skip(1)
                .Union(new string[]{projectName})
                .Reverse()
                .Join(".");
        }

        public void Alter(CsProjFile file)
        {
            var @namespace = GetNamespace(_relativePath, file.ProjectName);

            var filename = file.FileName.ParentDirectory().AppendPath(_relativePath);
            if (!filename.EndsWith(".cs"))
            {
                filename = filename + ".cs";
            }

            var text = _rawText
                .Replace(ASSEMBLY_NAME, file.ProjectName)
                .Replace(NAMESPACE, @namespace);

            new FileSystem().WriteStringToFile(filename, text);

            file.Add<CodeFile>(_relativePath);
        }
    }
}