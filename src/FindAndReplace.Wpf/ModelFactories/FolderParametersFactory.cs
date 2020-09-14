using System;
using System.Collections.Generic;
using FindAndReplace.Wpf.Backend.Collections;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class FolderParametersFactory
    {
        private static IEnumerable<String> GetDirectories(int count)
        {
            for (var index = 0; index < count; index++)
                yield return @$"C:\Example\Path\To\Folder{index + 1}";
        }

        private static IEnumerable<String> GetFileMasks(int count)
        {
            for (var index = 0; index < count; index++)
                yield return $"*.ext{index}";
        }

        public static FolderParameters GetFolderParameters(int seed)
        {
            var directories = GetDirectories(10);
            var fileExtensions = GetFileMasks(10);

            var fp = new FolderParameters
            {
                ExcludeDirectories = new ObservableHashSet<string>(directories),
                ExcludeFiles = new ObservableHashSet<string>(fileExtensions),
                IncludeFiles = new ObservableHashSet<string>(fileExtensions),
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
