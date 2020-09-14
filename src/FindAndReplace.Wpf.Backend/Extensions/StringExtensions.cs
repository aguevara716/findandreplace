using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FindAndReplace.Wpf.Backend.Extensions
{
    public static class StringExtensions
    {
        public static string ConvertWildcardPatternToRegexPattern(this string wildcardPattern)
        {
            var regexPattern = Regex.Escape(wildcardPattern)
                                    .Replace("\\*", ".*")
                                    .Replace("\\?", ".");

            return $"^{regexPattern}$";
        }

        public static bool IsEmpty(this string @string)
        {
            return @string.Length == 0;
        }

        public static string NormalizeNewlines(this string @string)
        {
            var normalizedString = Regex.Replace(@string, "\r\n|\n\r|\n|\r", Environment.NewLine);
            return normalizedString;
        }

        public static string[] SplitAndTrim(this string @string, string separator)
        {
            if (string.IsNullOrEmpty(separator))
                return new[] { @string.Trim() };

            var stringArray = @string.Split(separator).TrimItemsInArray();
            return stringArray;
        }

        public static string[] SplitOnNewline(this string @string)
        {
            var lines = @string.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            return lines;
        }

        public static string[] TrimItemsInArray(this string[] stringArray)
        {
            var trimmedArray = stringArray.Select(s => s.Trim()).ToArray();
            return trimmedArray;
        }

    }
}
