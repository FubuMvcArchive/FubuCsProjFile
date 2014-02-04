using System;
using FubuCore;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class ProjectReference : ProjectItem
    {

        public ProjectReference() : base("ProjectReference")
        {
        }

        public ProjectReference(string include)
            : base("ProjectReference", include)
        {
        }

        public Guid ProjectGuid { get; set; }
        public string ProjectName { get; set; }

        /*
         * 
    <ProjectReference Include="..\FubuCsProjFile\FubuCsProjFile.csproj">
      <Project>{5630FC3F-8C3E-4EAD-B960-B38FE3D87463}</Project>
      <Name>FubuCsProjFile</Name>
    </ProjectReference>
         * 
         */

        internal override MSBuildItem Configure(MSBuildItemGroup @group)
        {
            var item = base.Configure(@group);

            this.UpdateMetadata();

            return item;
        }

        internal override void Read(MSBuildItem item)
        {
            base.Read(item);

            ProjectName = item.GetMetadata("Name");
            var raw = item.GetMetadata("Project").TrimStart('{').TrimEnd('}');
            ProjectGuid = Guid.Parse(raw);
        }

        internal override void Save()
        {
            base.Save();
            this.UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            this.BuildItem.SetMetadata("Project", "{{{0}}}".ToFormat(ProjectGuid));
            if (ProjectName != null)
            {
                this.BuildItem.SetMetadata("Name", ProjectName);
            }
        }
    }
}