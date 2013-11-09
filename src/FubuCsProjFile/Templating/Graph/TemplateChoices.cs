using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCsProjFile.Templating.Graph
{
    /// <summary>
    /// Only used to build out one ProjectRequest at a time
    /// </summary>
    public class TemplateChoices
    {
        public string Tag;
        public string SetName;
        public string ProjectName;
        
        public IEnumerable<string> Options;
        public Cache<string, string> Selections = new Cache<string, string>(); 

        public readonly Substitutions Substitutions = new Substitutions();
    }
}