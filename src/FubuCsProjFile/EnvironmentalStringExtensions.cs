using System;
using FubuCore;

namespace FubuCsProjFile
{
    public static class EnvironmentalStringExtensions
    {
        private static readonly string[] Splitters = { "\r\n", "\n" };
        public static string[] SplitOnNewLine(this string value)
        {
            return value.Split(Splitters, StringSplitOptions.None);
        }

        public static string CanonicalPath(this string path)
        {
            return path.ToFullPath().ToLower().Replace("\\", "/");
        }
    }
}