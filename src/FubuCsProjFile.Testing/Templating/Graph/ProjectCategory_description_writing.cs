using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuCsProjFile.Templating.Graph;
using NUnit.Framework;

namespace FubuCsProjFile.Testing.Templating.Graph
{
    [TestFixture]
    public class ProjectCategory_description_writing
    {
        [Test]
        public void smoke_test()
        {
            var category = new ProjectCategory
            {
                Type = "new"
            };

            category.Templates.Add(new ProjectTemplate
            {
                Name = "web-app",
                Description = "FubuMVC application",
                Options = new List<Option>
                {
                    new Option("raven").DescribedAs("RavenDb database integration"),
                    new Option("authentication").DescribedAs("FubuMVC.Authentication integration"),
                    new Option("validation").DescribedAs("FubuMVC.Validation integration")
                    
                },
                Selections = new List<OptionSelection>
                {
                    new OptionSelection{Name = "view-engine", Description = "Choice of view engine", Options = new List<Option>
                    {
                        new Option("none").DescribedAs("No view engine"),
                        new Option("spark").DescribedAs("Spark Views"),
                        new Option("razor").DescribedAs("Razor Views")
                    }},
                    new OptionSelection{Name = "ioc", Description = "IoC container", Options = new List<Option>
                    {
                        new Option("structuremap").DescribedAs("StructureMap"),
                        new Option("autofac").DescribedAs("Autofac"),
                    }},
                }
            });

            category.Templates.Add(new ProjectTemplate
            {
                Name = "library",
                Description = "Simple class library project"
            });

            category.Templates.Add(new ProjectTemplate
            {
                Name = "web-bottle",
                Description = "FubuMVC Bottle",
                Options = new List<Option>
                {
                    new Option("raven").DescribedAs("RavenDb database integration"),
                    new Option("validation").DescribedAs("FubuMVC.Validation integration")
                },
                Selections = new List<OptionSelection>
                {
                    new OptionSelection{Name = "view-engine", Description = "Choice of view engine", Options = new List<Option>
                    {
                        new Option("none").DescribedAs("No view engine"),
                        new Option("spark").DescribedAs("Spark Views"),
                        new Option("razor").DescribedAs("Razor Views")
                    }}
                }
            });


            category.WriteDescriptionToConsole();
        }
    }
}