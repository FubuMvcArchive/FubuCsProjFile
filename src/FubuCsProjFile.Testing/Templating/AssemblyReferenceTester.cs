using System.Security.Cryptography.X509Certificates;
using FubuCsProjFile.Templating;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace FubuCsProjFile.Testing.Templating
{
    [TestFixture]
    public class AssemblyReferenceTester
    {
        [Test]
        public void can_write_assembly_reference_to_a_project()
        {
            var theContext = TemplatePlan.CreateClean("assembly-ref");

            theContext.Add(new CreateSolution("MySolution"));
            var projectPlan = new ProjectPlan("MyProject");
            theContext.Add(projectPlan);

            projectPlan.Add(new SystemReference("System.Configuration"));

            theContext.Execute();


            var project = CsProjFile.LoadFrom("assembly-ref".AppendPath("src","MyProject", "MyProject.csproj"));

            project.Find<AssemblyReference>("System.Configuration")
                   .ShouldNotBeNull();
        }

        [Test]
        public void can_write_and_read_fusion_name()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                FusionName = "some fusion"
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .FusionName.ShouldEqual("some fusion");
        }

        [Test]
        public void can_write_and_read_Aliases()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                Aliases = "some alias"
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .Aliases.ShouldEqual("some alias");
        }

        [Test]
        public void can_write_and_read_display_name()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                DisplayName = "some name"
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .DisplayName.ShouldEqual("some name");
        }

        [Test]
        public void can_write_and_read_SpecificVersion_true()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                SpecificVersion = true
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .SpecificVersion.Value.ShouldBeTrue();
        }

        [Test]
        public void can_write_and_read_SpecificVersion_false()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                SpecificVersion = false
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .SpecificVersion.Value.ShouldBeFalse();
        }

        [Test]
        public void can_write_and_read_SpecificVersion_null()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                SpecificVersion = null
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .SpecificVersion.ShouldBeNull();
        }


        [Test]
        public void can_write_and_read_Private_true()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                Private = true
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .Private.Value.ShouldBeTrue();
        }

        [Test]
        public void can_write_and_read_Private_false()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                Private = false
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .Private.Value.ShouldBeFalse();
        }

        [Test]
        public void can_write_and_read_Private_null()
        {
            var project = CsProjFile.CreateAtLocation("Foo.csproj", "Foo");
            var assemblyReference = new AssemblyReference("Foo")
            {
                Private = null
            };

            project.Add<AssemblyReference>(assemblyReference);

            project.Save();

            var project2 = CsProjFile.LoadFrom("Foo.csproj");

            project2.Find<AssemblyReference>("Foo")
                .Private.ShouldBeNull();
        }
    }
}