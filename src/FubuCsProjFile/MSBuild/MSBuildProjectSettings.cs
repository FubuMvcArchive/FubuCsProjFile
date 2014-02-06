namespace FubuCsProjFile.MSBuild
{
    public class MSBuildProjectSettings
    {
        /// <summary>
        /// When saving the project file, do we order the nodes
        /// in ascending order?
        /// </summary>
        public bool MaintainOriginalItemOrder { get; set; }

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
                    MaintainOriginalItemOrder = true
                };
            }
        }
    }
}