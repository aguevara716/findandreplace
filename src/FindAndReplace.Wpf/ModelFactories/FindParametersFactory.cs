using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class FindParametersFactory
    {
        public static FindParameters GetFindParameters(int seed)
        {
            var fp = new FindParameters
            {
                FindString = $"(Find Text #{seed}) Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                IsCaseSensitive = seed % 2 == 0,
                IsOnlyShowingFilesWithoutMatches = seed % 2 == 0,
                IsRegex = seed % 2 == 0,
                IsSearchingFilenameOnly = seed % 2 == 0,
                IsShowingErrors = seed % 2 == 0,
                IsUsingEscapeCharacters = seed % 2 == 0
            };
            return fp;
        }

        public static IEnumerable<FindParameters> GetFindParametersCollection(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetFindParameters(index + 1);
        }

    }
}
