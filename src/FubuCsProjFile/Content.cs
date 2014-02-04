using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class Content : ProjectItem
    {
        public static readonly string CopyToOutputDirectoryAtt = "CopyToOutputDirectory";

        public Content() : base("Content")
        {
            CopyToOutputDirectory = ContentCopy.Never;
        }

        public Content(string include) : base("Content", include)
        {
            CopyToOutputDirectory = ContentCopy.Never;
        }

        protected Content(string buildAction, string include) : base(buildAction, include)
        {
            CopyToOutputDirectory = ContentCopy.Never;
        }

        public ContentCopy CopyToOutputDirectory { get; set; }

        internal override MSBuildItem Configure(MSBuildItemGroup @group)
        {
            var item = base.Configure(@group);

            switch (CopyToOutputDirectory)
            {
                case ContentCopy.Always:
                    item.SetMetadata(CopyToOutputDirectoryAtt, "Always");
                    break;

                case ContentCopy.IfNewer:
                    item.SetMetadata(CopyToOutputDirectoryAtt, "PreserveNewest");
                    break;
            }

            return base.Configure(@group);
        }

        internal override void Read(MSBuildItem item)
        {
            base.Read(item);

            var copyString = item.HasMetadata(CopyToOutputDirectoryAtt)
                ? item.GetMetadata(CopyToOutputDirectoryAtt)
                : null;

            switch (copyString)
            {
                case null:
                    CopyToOutputDirectory = ContentCopy.Never;
                    break;

                case "Always":
                    CopyToOutputDirectory = ContentCopy.Always;
                    break;

                case "PreserveNewest":
                    CopyToOutputDirectory = ContentCopy.IfNewer;
                    break;
            }
        }
    }

    public enum ContentCopy
    {
        Always,
        Never,
        IfNewer
    }
}