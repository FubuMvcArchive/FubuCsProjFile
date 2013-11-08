using System;
using System.Linq;
using System.Runtime.Versioning;
using FubuCsProjFile.MSBuild;

namespace FubuCsProjFile
{
    public class FrameworkNameDetector
    {
        public const string DefaultIdentifier = ".NETFramework";
        public const string DefaultFrameworkVersion = "v4.0";

        public static FrameworkName Detect(MSBuildProject project)
        {
            var group = project.PropertyGroups.FirstOrDefault(x => x.Properties.Any(p => p.Name.Contains("TargetFramework")));
            var identifier = DefaultIdentifier;
            var versionString = DefaultFrameworkVersion;
            string profile = null;

            if (group != null)
            {
                identifier = group.GetPropertyValue("TargetFrameworkIdentifier") ?? DefaultIdentifier;
                versionString = group.GetPropertyValue("TargetFrameworkVersion") ?? DefaultFrameworkVersion;
                profile = group.GetPropertyValue("TargetFrameworkProfile");
            }

            var version = Version.Parse(versionString.Replace("v", "").Replace("V", ""));

            return new FrameworkName(identifier, version, profile);
        }
    }
}