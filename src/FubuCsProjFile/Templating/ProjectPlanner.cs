using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class ProjectPlanner : TemplatePlanner
    {
        public static readonly string NugetFile = "nuget.txt";

        public ProjectPlanner()
        {
            Matching(FileSet.Shallow(ProjectPlan.TemplateFile)).Do = (file, plan) => {
                plan.CurrentProject.ProjectTemplateFile = file.Path; };

            Matching(FileSet.Shallow(NugetFile)).Do = (file, plan) => {
                file.ReadLines()
                    .Where(x => FubuCore.StringExtensions.IsNotEmpty(x))
                    .Each(line => plan.CurrentProject.NugetDeclarations.Add(line.Trim()));
            };

            Matching(FileSet.Shallow(AssemblyInfoAlteration.SourceFile)).Do = (file, plan) => {
                var additions = file.ReadLines().Where(x => x.IsNotEmpty()).ToArray();
                plan.CurrentProject.Add(new AssemblyInfoAlteration(additions));
            };

            Matching(FileSet.Shallow(SystemReference.SourceFile)).Do = (file, plan) => {
                file.ReadLines()
                    .Where(x => x.IsNotEmpty())
                    .Each(assem => plan.CurrentProject.Add(new SystemReference(assem)));
            };

            /*
             * TODO: 
             * // only looks for a csproj.xml file
             * cs proj files
             * assembly references
             */
        }

        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            ProjectDirectory.PlanForDirectory(directory).Each(plan.CurrentProject.Add);

        }
    }
}