using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class SolutionProjectTester
    {
        [Test]
        public void can_parse_project_sections()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));
            solutionProject.ProjectSections.OfType<ProjectDependenciesSection>().Count().ShouldEqual(1);
        }

        [Test]
        public void can_write_project_sections_no_modification()
        {
            File.Copy("FubuMVC.SlickGrid.sln", "FubuMVC.SlickGrid.Copy.sln", true);
            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.Copy.sln");
            solution.Save();

            File.ReadAllText("FubuMVC.SlickGrid.Copy.sln").ShouldEqual(File.ReadAllText("FubuMVC.SlickGrid.sln"));
        }

        [Test]
        public void telling_solution_not_to_save_projects_should_not_save_projects()
        {
            var fileSystem = new FileSystem();
            var fullPathToHarnessProject = fileSystem.GetFullPath(@"SlickGridHarness\SlickGridHarness.csproj");

            var originalWriteTimestamp = new FileInfo(fullPathToHarnessProject).LastWriteTime;

            var solution = Solution.LoadFrom("FubuMVC.SlickGrid.sln");
            solution.Save(saveProjects: false);

            new FileInfo(fullPathToHarnessProject).LastWriteTime.ShouldEqual(originalWriteTimestamp);
        }

        [Test]
        public void can_add_build_dependencies_to_project_with_existing_dependencies()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));
            var newDependency = Guid.NewGuid();
            solutionProject.AddProjectDependency(newDependency);
            solutionProject.ProjectDependenciesSection.Dependencies.ShouldContain(newDependency);
        }

        [Test]
        public void can_not_add_same_build_dependencies_more_than_once()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));
            var newDependency = Guid.NewGuid();
            solutionProject.AddProjectDependency(newDependency);
            var depCount = solutionProject.ProjectDependenciesSection.Dependencies.Count;
            
            solutionProject.AddProjectDependency(newDependency);

            solutionProject.ProjectDependenciesSection.Dependencies.Count.ShouldEqual(depCount);
        }

        [Test]
        public void can_add_build_dependency_to_project_with_no_existing_section()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid"));
            var newDependency1 = Guid.NewGuid();
            var newDependency2 = Guid.NewGuid();
            solutionProject.AddProjectDependency(newDependency1);
            solutionProject.AddProjectDependency(newDependency2);

            solutionProject.ProjectDependenciesSection.Dependencies.ShouldContain(newDependency1);
            solutionProject.ProjectDependenciesSection.Dependencies.ShouldContain(newDependency2);            
        }

        [Test]
        public void can_remove_a_build_dependencies()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));
            
            solutionProject.RemoveProjectDependency(new Guid("A67A0CE1-E4C2-45FC-9019-829D434B2CC4"));
            solutionProject.ProjectDependenciesSection.ShouldEqual(null);
        }

        [Test]
        public void to_string_should_provide_useful_information_including_project_guid()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));
            solutionProject.ToString().ShouldContain(solutionProject.ProjectGuid.ToString("B").ToUpper());            
        }

        [Test]
        public void to_string_should_provide_useful_information_including_project_name()
        {
            var solutionProject = Solution.LoadFrom("FubuMVC.SlickGrid.sln").Projects.First(item => item.ProjectName.Equals("FubuMVC.SlickGrid.Testing"));            
            solutionProject.ToString().ShouldContain(solutionProject.ProjectName);            
        }
    }
}