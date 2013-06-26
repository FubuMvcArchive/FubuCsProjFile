using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuCsProjFile.Templating
{
    public class ProjectPlan : ITemplateStep
    {
        public const string NAMESPACE = "%NAMESPACE%";
        public const string ASSEMBLY_NAME = "%ASSEMBLY_NAME%";
        public const string PROJECT_PATH = "%PROJECT_PATH%";
        public static readonly string TemplateFile = "csproj.xml";

        private readonly string _projectName;
        private readonly IList<IProjectAlteration> _alterations = new List<IProjectAlteration>(); 
        private readonly IList<string> _nugetDeclarations = new List<string>();
        private string _relativePath;

        public ProjectPlan(string projectName)
        {
            _projectName = projectName;
        }

        public void Alter(TemplatePlan plan)
        {
            plan.Logger.StartProject(_alterations.Count);

            var reference = plan.Solution.FindProject(_projectName);
            if (reference == null)
            {
                if (ProjectTemplateFile.IsEmpty())
                {
                    plan.Logger.Trace("Creating project {0} from the default template", _projectName);
                    reference = plan.Solution.AddProject(_projectName);
                }
                else
                {
                    plan.Logger.Trace("Creating project {0} from template at {1}", _projectName, ProjectTemplateFile);
                    reference = plan.Solution.AddProjectFromTemplate(_projectName, ProjectTemplateFile);
                }
            }

            _relativePath = reference.Project.FileName.PathRelativeTo(plan.Root).Replace("\\", "/");

            _alterations.Each(x => {
                plan.Logger.TraceAlteration(x);
                x.Alter(reference.Project, this);
            });

            plan.Logger.EndProject();
        }

        public IList<string> NugetDeclarations
        {
            get { return _nugetDeclarations; }
        }

        public string ProjectTemplateFile { get; set; }

        public void Add(IProjectAlteration alteration)
        {
            _alterations.Add(alteration);
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public IList<IProjectAlteration> Alterations
        {
            get { return _alterations; }
        }

        public string ToNugetImportStatement()
        {
            return "{0}: {1}".ToFormat(ProjectName, _nugetDeclarations.OrderBy(x => x).Join(", "));
        }

        public string ApplySubstitutions(string rawText, string relativePath = null)
        {
            var builder = new StringBuilder(rawText);
            ApplySubstitutions(relativePath, builder);

            return builder.ToString();
        }

        internal void ApplySubstitutions(string relativePath, StringBuilder builder)
        {
            builder.Replace(ASSEMBLY_NAME, ProjectName);
            builder.Replace(PROJECT_PATH, _relativePath);

            if (relativePath.IsNotEmpty())
            {
                var @namespace = GetNamespace(relativePath, ProjectName);
                builder.Replace(NAMESPACE, @namespace);
            }
        }

        public static string GetNamespace(string relativePath, string projectName)
        {
            return relativePath
                .Split('/')
                .Reverse()
                .Skip(1)
                .Union(new string[] { projectName })
                .Reverse()
                .Join(".");
        }

        public override string ToString()
        {
            return "Create or load project '{0}'".ToFormat(_projectName);
        }
    }
}