using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class ReplaceParametersFactory
    {
        public static ReplaceParameters GetReplaceParameters(int seed)
        {
            var rp = new ReplaceParameters
            {
                IsRetainingModifiedDate = seed % 2 == 0,
                ReplaceString = $"(Replace Text #{seed}) Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            };
            return rp;
        }

        public static IEnumerable<ReplaceParameters> GetReplaceParametersCollection(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetReplaceParameters(index + 1);
        }

    }
}
