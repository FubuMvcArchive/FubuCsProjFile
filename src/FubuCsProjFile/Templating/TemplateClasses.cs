using System;

namespace FubuCsProjFile.Templating
{
    public class NugetReference : ITemplateStep
    {
        public string ProjectName { get; set; }
        public string NugetName { get; set; }
        public string Version { get; set; }

        public void Alter(TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }


    public class TemplateBuilder
    {
        /*
         * .cs files are CodeFileTemplate
         * description.txt is ignored
         * ignore.txt gets written to .gitignore in the solution
         * nuget.txt gets added to ripple and/or nuget dependencies
         * gems.txt gets added to Gemfile
         * files we don't recognize just get copied
         * references.txt <-- adds system assembly references
         * csproj.xml -- csproj
    
         * ****AssemblyInfo.cs is combined
         */

        public void ConfigureTree(string directory, TemplateContext context)
        {
            GemReference.Configure(directory, context);
        }

        public void ConfigureProject(string directory, ProjectPlan projectPlan, TemplateContext context)
        {
            throw new NotImplementedException();
        }
    }

    
}