using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            if (string.IsNullOrEmpty(rootDirectory))
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "Root directory must be specified");

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

            FileDiscoveryResult errorFileDiscoveryResult = null;
            var filesInDirectoryDictionary = new ConcurrentDictionary<string, bool>();
            Parallel.ForEach(fileMasks, fileMask =>
            {
                var fdr = GetFiles(rootDirectory, fileMask, enumerationOptions);
                if (!fdr.IsSuccessful)
                {
                    errorFileDiscoveryResult = fdr;
                    return;
                }

                foreach (var file in fdr.Files)
                {
                    filesInDirectoryDictionary.AddOrUpdate(file, true, (s, b) => true);
                }
            });
            if (errorFileDiscoveryResult != null)
                return errorFileDiscoveryResult;

            var filesInDirectoryCollection = filesInDirectoryDictionary.Keys.ToList();
            var filesMatchingMasksResult = FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectoryCollection);
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
