using System;
using System.IO;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuCsProjFile
{
    public class AssemblyInfo : CodeFile
    {
        private readonly CodeFile _codeFile;
        private readonly CsProjFile _projFile;
        private readonly FileSystem _fileSystem;
        public AssemblyInfo(CodeFile codeFile, CsProjFile projFile)
        {
            _codeFile = codeFile;
            _projFile = projFile;
            this._fileSystem = new FileSystem();
            this.Initialize();
        }

        internal override void Save()
        {
            if (this._fileSystem.FileExists(this.FullPath))
            {
                var result = new StringBuilder();
                
                this.UpdateLine(Lines, "AssemblyVersion", this.AssemblyVersion.ToString());
                this.UpdateLine(Lines, "AssemblyFileVersion", this.AssemblyFileVersion.ToString());
                this.UpdateLine(Lines, "AssemblyTitle", this.AssemblyTitle);
                this.UpdateLine(Lines, "AssemblyDescription", this.AssemblyDescription);
                this.UpdateLine(Lines, "AssemblyConfiguration", this.AssemblyConfiguration);
                this.UpdateLine(Lines, "AssemblyCompany", this.AssemblyCompany);
                this.UpdateLine(Lines, "AssemblyProduct", this.AssemblyProduct);
                this.UpdateLine(Lines, "AssemblyCopyright", this.AssemblyCopyright);
                
                Array.ForEach(this.Lines, s => result.AppendLine(s));
                this._fileSystem.WriteStringToFile(this.FullPath, result.ToString());
            }
        }

        private string FullPath
        {
            get { return this._fileSystem.GetFullPath(Path.Combine(this._projFile.ProjectDirectory, this._codeFile.Include)); }
        }

        private string[] Lines { get; set; }

        private void Initialize()
        {
            if (this._fileSystem.FileExists(this.FullPath))
            {
                this.Lines = this._fileSystem.ReadStringFromFile(this.FullPath).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                this.Parse("AssemblyVersion", value => this.AssemblyVersion = new Version(value.ExtractVersion()), Lines);
                this.Parse("AssemblyFileVersion", value => this.AssemblyFileVersion = new Version(value.ExtractVersion()), Lines);
                this.Parse("AssemblyTitle", value => this.AssemblyTitle = GetValueBetweenQuotes(value), Lines);
                this.Parse("AssemblyDescription", value => this.AssemblyDescription = GetValueBetweenQuotes(value), Lines);
                this.Parse("AssemblyConfiguration", value => this.AssemblyConfiguration = GetValueBetweenQuotes(value), Lines);
                this.Parse("AssemblyCompany", value => this.AssemblyCompany = GetValueBetweenQuotes(value), Lines);
                this.Parse("AssemblyProduct", value => this.AssemblyProduct = GetValueBetweenQuotes(value), Lines);
                this.Parse("AssemblyCopyright", value => this.AssemblyCopyright = GetValueBetweenQuotes(value), Lines);
            }
        }

        private void UpdateLine(string[] lines, string property, string value)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (Match(property,lines[i]))
                {
                    lines[i] = UpdateValueBetweenQuotes(lines[i], value);
                    break;
                }
            }
        }

        private void Parse(string property, Action<string> action, string[] lines)
        {
            var rawValue = lines.FirstOrDefault(line => Match(property, line));
            if (!string.IsNullOrWhiteSpace(rawValue))
            {
                action.Invoke(rawValue);                
            }
        }

        private static bool Match(string property, string line)
        {
            if (line.Trim().StartsWith("//"))
            {
                return false;
            }

            return line.IndexOf(property, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        private string GetValueBetweenQuotes(string value)
        {
            var start = value.IndexOf('"') + 1;
            var end = value.IndexOf('"', start);

            return value.Substring(start, end - start);
        }
        private string UpdateValueBetweenQuotes(string line, string value)
        {
            var start = line.IndexOf('"') + 1;
            var end = line.IndexOf('"', start);

            return line.Substring(0, start) + value + line.Substring(end);
        }


        public Version AssemblyVersion { get; set; }

        public Version AssemblyFileVersion { get; set; }

        public string AssemblyTitle { get; set; }

        public string AssemblyDescription { get; set; }

        public string AssemblyConfiguration { get; set; }

        public string AssemblyCompany { get; set; }

        public string AssemblyProduct { get; set; }

        public string AssemblyCopyright { get; set; }
    }
}