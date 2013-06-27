using System;
using System.Text;
using FubuCore;
using FubuCore.Util;
using System.Linq;

namespace FubuCsProjFile.Templating
{
    public class Substitutions
    {
        private readonly Cache<string, string> _values = new Cache<string, string>();
 
        public void SetIfNone(string key, string value)
        {
            _values.Fill(key, value);
        }

        public void Set(string key, string value)
        {
            _values[key] = value;
        }

        public string ValueFor(string key)
        {
            return _values[key];
        }

        public void ReadFrom(string file)
        {
            new FileSystem().ReadTextFile(file, line => {
                if (line.IsEmpty()) return;

                var parts = line.Split('=');
                _values[parts.First()] = parts.Last();
            });
        }

        public void WriteTo(string file)
        {
            new FileSystem()
                .WriteToFlatFile(file, writer => _values.Each(writer.WriteProperty));
        }

        public string ApplySubstitutions(string rawText)
        {
            var builder = new StringBuilder(rawText);
            _values.Each((key, value) => builder.Replace(key, value));

            return builder.ToString();
        }
    }
}