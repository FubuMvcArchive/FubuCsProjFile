using System.Collections.Generic;

namespace FubuCsProjFile.SolutionFile
{
    public class SolutionFileVersioning
    {
        public const string DefaultVersion = VS2012;

        public const string VS2010 = "VS2010";
        public const string VS2012 = "VS2012";
        public const string VS2013 = "VS2013";

        static SolutionFileVersioning()
        {
            VersionLines = new Dictionary<string, string[]>
            {
                {VS2010, new []{ "Microsoft Visual Studio Solution File, Format Version 11.00", "# Visual Studio 2010" }},
                {VS2012, new[] { "Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio 2012" }},
                {VS2013, new[] { "Microsoft Visual Studio Solution File, Format Version 13.00", "# Visual Studio 2013" }}
            };
        }

        public static IDictionary<string, string[]> VersionLines { get; private set; } 
    }
}