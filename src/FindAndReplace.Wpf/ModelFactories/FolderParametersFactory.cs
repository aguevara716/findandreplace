using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class FolderParametersFactory
    {
        public static FolderParameters GetFolderParameters(int seed)
        {
            var fp = new FolderParameters
            {
                ExcludeDirectories = $"Exclude Directories {seed}",
                ExcludeMask = $"Exclude Mask {seed}",
                FileMask = $"File Mask {seed}",
                IsRecursive = seed % 2 == 0,
                RootDirectory = $"Root Directory {seed}"
            };
            return fp;
        }

        public static IEnumerable<FolderParameters> GetFolderParametersCollection(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetFolderParameters(index + 1);
        }

    }
}
