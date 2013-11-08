using System.Collections.Generic;

namespace FubuCsProjFile.Templating.Graph
{
    public class Template
    {
        public TemplateType Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }

        public IEnumerable<Input> Inputs()
        {
            return Input.ReadFrom(Path);
        } 
    }
}