using System.Diagnostics;
using System.IO;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class CopyProjectReferencesTester
    {
        private TemplatePlan thePlan;
        private CsProjFile theOriginalProject;
        private CsProjFile theTestingProject;
        private ProjectPlan testingPlan;

        [SetUp]
        public void SetUp()
        {
            thePlan = TemplatePlan.CreateClean("copy-references");
        
            thePlan.FileSystem.WriteStringToFile("ripple.dependencies.config", @"FubuCore
FubuMVC.Core
");

            thePlan.Add(new CreateSolution("References"));
            var originalPlan = new ProjectPlan("References");
            thePlan.Add(originalPlan);
            originalPlan.Add(new SystemReference("System.Data"));
            originalPlan.Add(new SystemReference("System.Configuration"));
            originalPlan.Add(new CopyFileToProject("ripple.dependencies.config", "ripple.dependencies.config"));
            originalPlan.NugetDeclarations.Add("Bottles");
            originalPlan.NugetDeclarations.Add("FubuMVC.Core");
            originalPlan.NugetDeclarations.Add("FubuLocalization");

            testingPlan = new ProjectPlan("References.Testing");
            thePlan.Add(testingPlan);
            thePlan.Add(new CopyProjectReferences("References"));
            

            thePlan.Execute();

            theOriginalProject = thePlan.Solution.FindProject("References").Project;
            theTestingProject = thePlan.Solution.FindProject("References.Testing").Project;
        }

        [Test]
        public void copies_the_system_references_from_the_parent()
        {
            theTestingProject.Find<AssemblyReference>("System.Data").ShouldNotBeNull();
            theTestingProject.Find<AssemblyReference>("System.Configuration").ShouldNotBeNull();
        }

        [Test]
        public void gathers_the_nugets_from_nugets_added_to_the_parent()
        {
            testingPlan.NugetDeclarations.ShouldContain("Bottles");
            testingPlan.NugetDeclarations.ShouldContain("FubuLocalization");
        }

        [Test]
        public void gathers_the_nugets_from_the_ripple_dependencies_file_of_the_original_project()
        {
            testingPlan.NugetDeclarations.ShouldContain("FubuCore");
        }

        [Test]
        public void should_apply_a_project_reference_to_the_original_project()
        {
            var reference = theTestingProject.All<ProjectReference>().Single();
            reference.ShouldNotBeNull();

            reference.Include.ShouldEqual(@"..\References\References.csproj");
            reference.ProjectName.ShouldEqual("References");
            reference.ProjectGuid.ShouldEqual(theOriginalProject.ProjectGuid);
        }

        [Test]
        public void should_have_written_out_a_ripple_install_file()
        {
            var path = thePlan.Root.AppendPath("ripple-install.txt");
            File.Exists(path).ShouldBeTrue();

            thePlan.AlterFile(path.ToFullPath(), list => {
                list.ShouldContain("References.Testing: Bottles, FubuCore, FubuLocalization, FubuMVC.Core");
            });
        }
    }
}