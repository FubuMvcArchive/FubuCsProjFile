using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile.ProjectFiles.CsProj
{
    public class CsProjCreator : IPojectCreatorForType
    {
        public MSBuildProject CreateMSBuildProject(string assemblyName)
        {
            return MSBuildProject.Create<CsProjFile>(assemblyName);
        }

        public IProjectFile Create(MSBuildProject project, string filename)
        {
            return new CsProjFile(filename, project);
        }
    }
}
