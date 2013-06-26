using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class CopyProjectReferences : ITemplateStep
    {
        private readonly string _originalProject;

        public CopyProjectReferences(string originalProject)
        {
            _originalProject = originalProject;
        }

        public void Alter(TemplatePlan plan)
        {
            var original = plan.Solution.FindProject(_originalProject).Project;
            var originalPlan = plan.FindProjectPlan(_originalProject);

            var testPlan = plan.CurrentProject;
            var testProject = plan.Solution.FindProject(testPlan.ProjectName).Project;

            copyNugetDeclarations(originalPlan, testPlan, original, testProject);

            findNugetsInOriginalRippleDeclarations(plan, testPlan);

            buildProjectReference(original, testProject);
        }

        private static void copyNugetDeclarations(ProjectPlan originalPlan, ProjectPlan testPlan, CsProjFile original,
                                                  CsProjFile testProject)
        {
            originalPlan.NugetDeclarations.Each(x => testPlan.NugetDeclarations.Fill(x));
            original.All<AssemblyReference>()
                    .Where(x => x.HintPath.IsEmpty())
                    .Each(x => testProject.Add<AssemblyReference>(x.Include));
        }

        private void findNugetsInOriginalRippleDeclarations(TemplatePlan plan, ProjectPlan testPlan)
        {
            var configFile = _originalProject.ParentDirectory().AppendPath("ripple.dependencies.config");
            plan.FileSystem.ReadTextFile(configFile, line => { if (line.IsNotEmpty()) testPlan.NugetDeclarations.Fill(line); });
        }

        private static void buildProjectReference(CsProjFile original, CsProjFile testProject)
        {
            var relativePathToTheOriginal = original.FileName.PathRelativeTo(testProject.FileName);
            if (original.FileName.ParentDirectory().ParentDirectory() ==
                testProject.FileName.ParentDirectory().ParentDirectory())
            {
                relativePathToTheOriginal = Path.Combine("..", Path.GetFileName(original.FileName.ParentDirectory()),
                                                         Path.GetFileName(original.FileName));
            }

            
            var reference = new ProjectReference(relativePathToTheOriginal)
            {
                ProjectGuid = original.ProjectGuid,
                ProjectName = original.ProjectName
            };

            testProject.Add(reference);
        }

        public override string ToString()
        {
            return string.Format("Copy all references from {0}", _originalProject);
        }
    }
}