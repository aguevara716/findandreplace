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

    }
}
