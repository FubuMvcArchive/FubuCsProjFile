using System.Xml;
using FubuCsProjFile.MSBuild;
using FubuCore;

namespace FubuCsProjFile
{
    public class AssemblyReference : ProjectItem
    {
        private const string HintPathAtt = "HintPath";

        public AssemblyReference() : base("Reference")
        {
        }

        public AssemblyReference(string assemblyName) : base("Reference", assemblyName)
        {
        }

        public AssemblyReference(string assemblyName, string hintPath) : this(assemblyName)
        {
            HintPath = hintPath;
        }

        public string HintPath { get; set; }

        internal override MSBuildItem Configure(MSBuildItemGroup @group)
        {
            var item = base.Configure(@group);
            if (HintPath.IsNotEmpty())
            {
                item.SetMetadata(HintPathAtt, HintPath);
            }

            return item;
        }

        internal override void Read(MSBuildItem item)
        {
            base.Read(item);


            HintPath = item.HasMetadata(HintPathAtt) ? item.GetMetadata(HintPathAtt) : null;
        }
    }
}