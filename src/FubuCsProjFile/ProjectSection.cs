using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace FubuCsProjFile
{
    /// <summary>
    /// This type represents the project section in a Visual Studio solution file
    /// </summary>
    /// <example>
    /// <code>
    /// Project("{8D1827A7-60BD-4138-A79E-54C8CF6B7198}") = "Foo.UnitTests", "Foo\Foo.UnitTests.csproj", "{0F137D9D-59F6-4CE4-B8F8-04AB7120EDF9}"
    ///     ProjectSection(ProjectDependencies) = postProject
    ///         {7E77EC80-4BE0-4B5F-B76A-069F7F2BF26D} = {7E77EC80-4BE0-4B5F-B76A-069F7F2BF26D}
    ///     EndProjectSection
    /// EndProject
    /// </code>
    /// In the above solution snippet, the project Foo.UnitTest defines a project dependency 
    /// to the project with guid {7E77EC80-4BE0-4B5F-B76A-069F7F2BF26D}. Which means Foo.UnitTest 
    /// will get build after {7E77EC80-4BE0-4B5F-B76A-069F7F2BF26D}
    /// </example>
    public class ProjectSection
    {
        protected readonly List<string> _properties = new List<string>();
        private readonly string _declaration;

        public ProjectSection(string declaration)            
        {
            _declaration = declaration.Trim();
        }

        public ReadOnlyCollection<string> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        public void Read(string text)
        {
            _properties.Add(text.Trim());
        }
        
        public void Write(StringWriter writer)
        {
            writer.WriteLine("\t" + _declaration);
            Properties.Each(x => writer.WriteLine("\t\t" + x));
            writer.WriteLine("\t" + "EndProjectSection");
        }
    }
}