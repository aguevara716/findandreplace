using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Filesystem
{
    public interface IFileRetriever
    {
        FileDiscoveryResult GetFiles(string rootDirectory, IList<string> fileMasks, bool isRecursive);
    }

    public class FileRetriever : IFileRetriever
    {
        public FileDiscoveryResult GetFiles(string rootDirectory, IList<string> fileMasks, bool isRecursive)
        {
            var enumerationOptions = new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                BufferSize = 0,
                IgnoreInaccessible = true,
                MatchCasing = MatchCasing.CaseInsensitive,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = isRecursive,
                ReturnSpecialDirectories = false
            };

            if (!fileMasks.Any())
                fileMasks.Add("*");

            var filesInDirectoryHashSet = new HashSet<string>();
            foreach (var fileMask in fileMasks)
            {
                var fdr = GetFiles(rootDirectory, fileMask, enumerationOptions);
                if (!fdr.IsSuccessful)
                    return fdr;

                filesInDirectoryHashSet.UnionWith(fdr.Files);
            }

            var filesMatchingMasksResult = FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectoryHashSet.ToList());
            return filesMatchingMasksResult;
        }

        private FileDiscoveryResult GetFiles(string rootDirectory, string fileMask, EnumerationOptions enumerationOptions)
        {
            try
            {
                var files = Directory.GetFiles(rootDirectory, fileMask, enumerationOptions);
                return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, files);
            }
            catch (Exception ex)
            {
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory,
                                                                              $"Failed to retrieve files matching \"{fileMask}\" in directory \"{rootDirectory}\"",
                                                                              ex);
            }
        }
    }
}
