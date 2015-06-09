using System.Xml;

namespace FubuCsProjFile.MSBuild
{
    public class MSBuildTarget : MSBuildObject    
    {
        public MSBuildTarget(XmlElement elem)
            : base(elem)
        {
        }
       
        public string Name
        {
            get { return Element.GetAttribute("Name"); }
        }
    }
}