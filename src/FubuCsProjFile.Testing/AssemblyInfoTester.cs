using System;
using FubuCore;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class AssemblyInfoTester
    {
        private FileSystem _fileSystem;
        private CsProjFile _project;
        public AssemblyInfoTester()
        {
            _fileSystem = new FileSystem();
        }

        [SetUp]
        public void BeforeEveryTest()
        {
            _fileSystem.Copy("SlickGridHarness.csproj.fake", @"AssemblyInfoTester\SlickGridHarness.csproj", CopyBehavior.overwrite);
            _fileSystem.Copy("AssemblyInfo.cs.fake", @"AssemblyInfoTester\Properties\AssemblyInfo.cs", CopyBehavior.overwrite);
            _project = CsProjFile.LoadFrom(@"AssemblyInfoTester\SlickGridHarness.csproj");
        }

        [Test]
        public void CanReadAllAttributes()
        {
            var assemblyInfo = _project.AssemblyInfo;

            Assert.That(assemblyInfo.AssemblyVersion, Is.EqualTo(new Version(1, 0, 0, 0)));
            Assert.That(assemblyInfo.AssemblyFileVersion, Is.EqualTo(new Version(1, 0, 0, 200)));
            Assert.That(assemblyInfo.AssemblyTitle, Is.EqualTo("FubuCsProjFile.Testing"));
            Assert.That(assemblyInfo.AssemblyDescription, Is.EqualTo("FubuCsProjFile.Testing Is Amazing"));
            Assert.That(assemblyInfo.AssemblyConfiguration, Is.EqualTo("Debug build on machine build02"));
            Assert.That(assemblyInfo.AssemblyCompany, Is.EqualTo("Fubu"));
            Assert.That(assemblyInfo.AssemblyProduct, Is.EqualTo("FubuCsProjFile.Testing Product"));
            Assert.That(assemblyInfo.AssemblyCopyright, Is.EqualTo("Copyright ©  2014"));
            Assert.That(assemblyInfo.AssemblyInformationalVersion, Is.EqualTo("Release AnyCPU / f45ee8c"));
        }


        [Test]
        public void CanReadAttributes_when_some_attributes_are_missing()
        {
            _fileSystem.Copy("AssemblyInfoWithMissingFileVersionAttribute.cs.fake", @"AssemblyInfoTester\Properties\AssemblyInfo.cs", CopyBehavior.overwrite);
            _project = CsProjFile.LoadFrom(@"AssemblyInfoTester\SlickGridHarness.csproj");
            var assemblyInfo = _project.AssemblyInfo;


            Assert.That(assemblyInfo.AssemblyVersion, Is.EqualTo(new Version(1, 0, 0, 0)));
            Assert.That(assemblyInfo.AssemblyTitle, Is.EqualTo("FubuCsProjFile.Testing"));
            Assert.That(assemblyInfo.AssemblyDescription, Is.EqualTo("FubuCsProjFile.Testing Is Amazing"));
            Assert.That(assemblyInfo.AssemblyConfiguration, Is.EqualTo("Debug build on machine build02"));
            Assert.That(assemblyInfo.AssemblyCompany, Is.EqualTo("Fubu"));
            Assert.That(assemblyInfo.AssemblyProduct, Is.EqualTo("FubuCsProjFile.Testing Product"));
            Assert.That(assemblyInfo.AssemblyCopyright, Is.EqualTo("Copyright ©  2014"));
            Assert.That(assemblyInfo.AssemblyInformationalVersion, Is.Null);
        }

        [Test]
        public void CanWriteAllAtrributes()
        {
            var assemblyInfo = _project.AssemblyInfo;
            assemblyInfo.AssemblyVersion = new Version(2, 0, 0, 0);
            assemblyInfo.AssemblyFileVersion = new Version(2, 0, 0, 120);
            assemblyInfo.AssemblyTitle = "New AssemblyTitle";
            assemblyInfo.AssemblyDescription = "New AssemblyDescription";
            assemblyInfo.AssemblyConfiguration = "New AssemblyConfiguration";
            assemblyInfo.AssemblyCompany = "New AssemblyCompany";
            assemblyInfo.AssemblyProduct = "New AssemblyProduct";
            assemblyInfo.AssemblyCopyright = "New AssemblyCopyright";
            assemblyInfo.AssemblyInformationalVersion = "New AssemblyInformationalVersion";

            _project.Save();

            _project = CsProjFile.LoadFrom(@"AssemblyInfoTester\SlickGridHarness.csproj");
            assemblyInfo = _project.AssemblyInfo;

            Assert.That(assemblyInfo.AssemblyVersion, Is.EqualTo(new Version(2, 0, 0, 0)));
            Assert.That(assemblyInfo.AssemblyFileVersion, Is.EqualTo(new Version(2, 0, 0, 120)));
            Assert.That(assemblyInfo.AssemblyTitle, Is.EqualTo("New AssemblyTitle"));
            Assert.That(assemblyInfo.AssemblyDescription, Is.EqualTo("New AssemblyDescription"));
            Assert.That(assemblyInfo.AssemblyConfiguration, Is.EqualTo("New AssemblyConfiguration"));
            Assert.That(assemblyInfo.AssemblyCompany, Is.EqualTo("New AssemblyCompany"));
            Assert.That(assemblyInfo.AssemblyProduct, Is.EqualTo("New AssemblyProduct"));
            Assert.That(assemblyInfo.AssemblyCopyright, Is.EqualTo("New AssemblyCopyright"));
            Assert.That(assemblyInfo.AssemblyInformationalVersion, Is.EqualTo("New AssemblyInformationalVersion"));
        }

        [Test]
        public void CanWriteEvenWhenFileVersionNotSpecified()
        {
            _fileSystem.Copy("AssemblyInfoWithMissingFileVersionAttribute.cs.fake", @"AssemblyInfoTester\Properties\AssemblyInfo.cs", CopyBehavior.overwrite);
            _project = CsProjFile.LoadFrom(@"AssemblyInfoTester\SlickGridHarness.csproj");

            var assemblyInfo = _project.AssemblyInfo;
            assemblyInfo.AssemblyVersion = new Version(2, 0, 0, 0);
            assemblyInfo.AssemblyTitle = "New AssemblyTitle";
            assemblyInfo.AssemblyDescription = "New AssemblyDescription";
            assemblyInfo.AssemblyConfiguration = "New AssemblyConfiguration";
            assemblyInfo.AssemblyCompany = "New AssemblyCompany";
            assemblyInfo.AssemblyProduct = "New AssemblyProduct";
            assemblyInfo.AssemblyCopyright = "New AssemblyCopyright";

            _project.Save();

            _project = CsProjFile.LoadFrom(@"AssemblyInfoTester\SlickGridHarness.csproj");
            assemblyInfo = _project.AssemblyInfo;

            Assert.That(assemblyInfo.AssemblyVersion, Is.EqualTo(new Version(2, 0, 0, 0)));
            Assert.That(assemblyInfo.AssemblyFileVersion, Is.Null);
            Assert.That(assemblyInfo.AssemblyTitle, Is.EqualTo("New AssemblyTitle"));
            Assert.That(assemblyInfo.AssemblyDescription, Is.EqualTo("New AssemblyDescription"));
            Assert.That(assemblyInfo.AssemblyConfiguration, Is.EqualTo("New AssemblyConfiguration"));
            Assert.That(assemblyInfo.AssemblyCompany, Is.EqualTo("New AssemblyCompany"));
            Assert.That(assemblyInfo.AssemblyProduct, Is.EqualTo("New AssemblyProduct"));
            Assert.That(assemblyInfo.AssemblyCopyright, Is.EqualTo("New AssemblyCopyright"));
        }
    }
}