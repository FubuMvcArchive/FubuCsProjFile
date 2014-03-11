using FubuCsProjFile.ProjectFiles.CsProj;

namespace FubuCsProjFile.MSBuild
{
    public class MSBuildProjectSettings
    {
        /// <summary>
        /// When saving the project file, do we order the nodes
        /// in ascending order?
        /// </summary>
        public bool MaintainOriginalItemOrder { get; set; }

        /// <summary>
        /// When calls are made to <see cref="CsProjFile.Save()"/>, only
        /// save the file if the project differs from the one on disk.
        /// </summary>
        public bool OnlySaveIfChanged { get; set; }

        public static MSBuildProjectSettings DefaultSettings
        {
            get
            {
                return new MSBuildProjectSettings
                {
                    MaintainOriginalItemOrder = false
                };
            }
        }

        public static MSBuildProjectSettings MinimizeChanges
        {
            get
            {
                return new MSBuildProjectSettings
                {
                    MaintainOriginalItemOrder = true,
                    OnlySaveIfChanged = true
                };
            }
        }
    }
}