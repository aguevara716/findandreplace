using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface ITextMatcher
    {
        TextMatcherResult FindTextInFile(string filePath,
                                         string findText,
                                         string content,
                                         bool isRegexSearch,
                                         bool isUsingEscapeCharacters,
                                         bool isCaseSensitive);
    }

    public class TextMatcher : ITextMatcher
    {
        public TextMatcherResult FindTextInFile(string filePath,
                                                string findText,
                                                string content,
                                                bool isRegexSearch,
                                                bool isUsingEscapeCharacters,
                                                bool isCaseSensitive)
        {
            if (string.IsNullOrEmpty(filePath))
                return TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, "File path is required");
            if (string.IsNullOrEmpty(findText))
                return TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, "Find text is required");
            if (content == null)
                return TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, "Content should not be null");
            if (content.IsEmpty())
                return TextMatcherResult.CreateSuccess<TextMatcherResult>(filePath, new List<TextMatch>());

            try
            {
                var regexOptions = RegexOptions.Multiline;
                if (!isCaseSensitive)
                    regexOptions |= RegexOptions.IgnoreCase;

                var regexFilter = !isRegexSearch && !isUsingEscapeCharacters
                    ? Regex.Escape(findText)
                    : findText;

                var textMatchesDictionary = new ConcurrentDictionary<TextMatch, bool>();
                var matches = Regex.Matches(content, regexFilter, regexOptions);
                Parallel.ForEach(matches, match =>
                {
                    var textMatch = new TextMatch
                    {
                        Length = match.Length,
                        StartIndex = match.Index
                    };
                    textMatchesDictionary.AddOrUpdate(textMatch, true, (t, b) => true);
                });

                var textMatchesList = textMatchesDictionary.Keys
                                                           .AsParallel()
                                                           .OrderBy(tm => tm.StartIndex)
                                                           .ThenBy(tm => tm.StartIndex + tm.Length)
                                                           .ToList();
                return TextMatcherResult.CreateSuccess<TextMatcherResult>(filePath, textMatchesList);
            }
            catch (Exception ex)
            {
                return TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, ex);
            }
        }

    }
}
