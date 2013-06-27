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
            ShallowMatch(Input.File).Do = (file, plan) => {
                var inputs = Input.ReadFromFile(file.Path);
                plan.CurrentProject.Substitutions.ReadInputs(inputs);
            };

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

            Matching(FileSet.Deep("*.cs")).Do = (file, plan) => {
                var template = new CodeFileTemplate(file.RelativePath, file.ReadAll());
                plan.CurrentProject.Add(template);
            };
        }

        protected override void configurePlan(string directory, TemplatePlan plan)
        {
            ProjectDirectory.PlanForDirectory(directory).Each(plan.CurrentProject.Add);
        }
    }
}