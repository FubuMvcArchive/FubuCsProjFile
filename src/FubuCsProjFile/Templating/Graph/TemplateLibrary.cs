using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuCsProjFile.Templating.Graph
{
    public class TemplateLibrary : ITemplateLibrary
    {
        public static readonly string Solution = "solution";
        public static readonly string Project = "project";
        public static readonly string Testing = "testing";
        public static readonly string Alteration = "alteration";

        private readonly Cache<TemplateType, string> _templateDirectories;


        public static readonly string DescriptionFile = "description.txt";
        public readonly static IFileSystem FileSystem = new FileSystem();

        public static TemplateLibrary BuildClean(string root)
        {
            FileSystem.DeleteDirectory(root);
            FileSystem.CreateDirectory(root);

            return new TemplateLibrary(root);
        }

        public TemplateBuilder StartTemplate(TemplateType type, string name)
        {
            var directory = _templateDirectories[type].AppendPath(name);
            return new TemplateBuilder(directory);
        }

        private readonly string _templatesRoot;


        public TemplateLibrary(string templatesRoot)
        {
            _templatesRoot = templatesRoot;
            _templateDirectories = new Cache<TemplateType, string>(type => {
                var directory = _templatesRoot.AppendPath(type.ToString().ToLowerInvariant());

                FileSystem.CreateDirectory(directory);

                return directory;
            });

            Enum.GetValues(typeof (TemplateType)).OfType<TemplateType>()
                .Each(x => _templateDirectories.FillDefault(x));
        }

        public IEnumerable<Template> All()
        {
            return _templateDirectories.GetAllKeys().SelectMany(readTemplates);
        }

        private IEnumerable<Template> readTemplates(TemplateType templateType)
        {
            var directory = _templateDirectories[templateType];
            foreach ( var templateDirectory in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
            {
                var template = new Template
                {
                    Name = Path.GetFileName(templateDirectory),
                    Path = templateDirectory,
                    Type = templateType
                };

                var descriptionFile = templateDirectory.AppendPath(DescriptionFile);
                if (FileSystem.FileExists(descriptionFile))
                {
                    template.Description = FileSystem.ReadStringFromFile(descriptionFile);
                }

                yield return template;
            }
        } 

        public Template Find(TemplateType type, string name)
        {
            return readTemplates(type).FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public IEnumerable<Template> Find(TemplateType type, IEnumerable<string> names)
        {
            return names.Select(x => Find(type, x));
        }

        public IEnumerable<MissingTemplate> Validate(TemplateType type, params string[] names)
        {
            var templates = readTemplates(type);

            foreach (var name in names)
            {
                if (templates.Any(x => x.Name.EqualsIgnoreCase(name)))
                {
                    continue;
                }

                yield return new MissingTemplate
                {
                    Name = name,
                    TemplateType = type,
                    ValidChoices = templates.Select(x => x.Name).ToArray()
                };
            }
        }
    }
}