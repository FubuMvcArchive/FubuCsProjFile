using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuCsProjFile
{
    public static class StringExtensions
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

        public static bool ContainsSequence(this List<string> list, IEnumerable<string> lines)
        {
            var index = list.IndexOf(lines.First());
            if (index == -1)
            {
                return false;
            }

            if (lines.Count() == 1) return true;

            int i = 0;
            foreach (var line in lines)
            {
                if (list.Count <= index + i) return false;

                if (list[index + i] != line) return false;

                i++;
            }

            return true;
        }

        public static string ExtractVersion(this string source)
        {
            var result = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                var value = source[i];

                if ((!Char.IsDigit(value) && value != '.') && result.Length > 0)
                    break;

                if (Char.IsDigit(value) || (value == '.' && result.Length > 0))
                {
                    result.Append(value);
                }
            }

            return result.ToString().TrimEnd('.');
        }
    }
}