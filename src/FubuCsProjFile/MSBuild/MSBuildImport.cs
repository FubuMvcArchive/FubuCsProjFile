using System.Xml;

namespace FubuCsProjFile.MSBuild
{
    public class MSBuildImport : MSBuildObject    
    {
        public MSBuildImport(XmlElement elem) : base(elem)
        {
        }

        public string Project
        {
            get { return Element.GetAttribute("Project"); }
        }

        public string Name
        {
            get { return Element.Name; }
        }
    }
}