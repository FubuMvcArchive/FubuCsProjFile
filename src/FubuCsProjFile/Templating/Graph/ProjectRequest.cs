using System;
using System.Collections.Generic;

namespace FubuCsProjFile.Templating.Graph
{
    public class ProjectRequest
    {
        public readonly IList<string> Alterations = new List<string>(); 
        private readonly Substitutions _substitutions = new Substitutions();

        public ProjectRequest(string name, string template)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (template == null) throw new ArgumentNullException("template");

            Name = name;
            Template = template;
        }

        public ProjectRequest(string name, string template, string originalProject)
            : this(name, template)
        {
            OriginalProject = originalProject;
        }

        public string Version = DotNetVersion.V40;
        public string Name { get; private set; }
        public string Template { get; private set; }

        /// <summary>
        /// For testing and extension projects
        /// </summary>
        public string OriginalProject;

        public Substitutions Substitutions
        {
            get { return _substitutions; }
        }

    }
}