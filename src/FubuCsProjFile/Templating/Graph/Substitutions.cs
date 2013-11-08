using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;
using FubuCore.Util;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;

namespace FubuCsProjFile.Templating.Graph
{
    public class Substitutions
    {
        public static readonly string ConfigFile = "fubu.templates.config";
        
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
                SetIfNone(parts.First(), parts.Last());
            });
        }

        public void WriteTo(string file)
        {
            new FileSystem()
                .WriteToFlatFile(file, writer => _values.Each((key, value ) => {
                    if (key != TemplatePlan.INSTRUCTIONS)
                    {
                        writer.WriteProperty(key, value);
                    }
                }));
        }

        public bool Has(string key)
        {
            return _values.Has(key);
        }

        public string ApplySubstitutions(string rawText, Action<StringBuilder> moreAlteration = null)
        {
            var builder = new StringBuilder(rawText);
            if (moreAlteration != null)
            {
                moreAlteration(builder);
            }

            ApplySubstitutions(builder);

            return builder.ToString();
        }

        public void ApplySubstitutions(StringBuilder builder)
        {
            _values.Each((key, value) => builder.Replace(key, value));
        }

        public void CopyTo(Substitutions substitutions2)
        {
            _values.Each(substitutions2.Set);
        }

        public void ReadInputs(IEnumerable<Input> inputs, Action<string> markMissing)
        {
            inputs.Each(x => {
                if (!_values.Has(x.Name) && x.Default.IsEmpty())
                {
                    markMissing(x.Name);
                }
                
                var resolved = ApplySubstitutions((x.Default ?? string.Empty));
                SetIfNone(x.Name, resolved);
            });
        }

        public void Trace(ITemplateLogger logger)
        {
            _values.Each((key, value) => {
                if (key != TemplatePlan.INSTRUCTIONS)
                {
                    logger.Trace("{0}={1}", key, value);
                }
            });
        }
    }
}