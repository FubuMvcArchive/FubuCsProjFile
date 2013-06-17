using FubuCore;
using FubuCsProjFile.Templating;

namespace FubuCsProjFile.Testing
{
    public class DataMother
    {
        private readonly string _directory;
        private TemplateContext _context;

        public DataMother(string directory)
        {
            _directory = directory;

            _context = TemplateContext.CreateClean(_directory);
        }

        public FileContents ToPath(params string[] pathParts)
        {
            var path = _directory.AppendPath(pathParts);
            return new FileContents(path);
        }

        public TemplateContext Context
        {
            get { return _context; }
        }

        public class FileContents
        {
            private readonly string _path;

            public FileContents(string path)
            {
                _path = path;
            }

            public void WriteContent(string content)
            {
                new FileSystem().WriteStringToFile(_path, content.TrimStart());
            }

            public void WriteEmpty()
            {
                WriteContent(string.Empty);
            }
        }

        public TemplateContext BuildSolutionPlan()
        {
            new TemplateBuilder().ConfigureTree(_directory, _context);

            return _context;
        }
    }

    
}