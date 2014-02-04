using FubuCore;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class CodeFile : ProjectItem
    {
        private const string LinkAtt = "Link";

        public CodeFile(string relativePath) : base("Compile", relativePath)
        {
        }

        public CodeFile() : base("Compile")
        {
        }

        public string Link { get; set; }

        internal override MSBuildItem Configure(MSBuildItemGroup @group)
        {
            var item = base.Configure(@group);
            this.UpdateMetadata();

            return item;
        }

        internal override void Read(MSBuildItem item)
        {
            base.Read(item);


            Link = item.HasMetadata(LinkAtt) ? item.GetMetadata(LinkAtt) : null;
        }

        internal override void Save()
        {
            base.Save();
            this.UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            if (Link.IsNotEmpty())
            {
                this.BuildItem.SetMetadata(LinkAtt, Link);
            }
        }
    }
}