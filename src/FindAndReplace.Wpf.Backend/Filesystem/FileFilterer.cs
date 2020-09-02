using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Filesystem
{
    public interface IFileFilterer
    {
        FileDiscoveryResult FilterOutExcludedDirectories(string rootDirectory,
                                                         IList<string> filesInDirectory,
                                                         IList<string> excludedDirectories,
                                                         bool isRecursive);
        FileDiscoveryResult FilterOutExcludedFileMasks(string rootDirectory,
                                                       IList<string> filesInDirectory,
                                                       IList<string> excludedFileMasks);
    }

    public class FileFilterer : IFileFilterer
    {
        public FileDiscoveryResult FilterOutExcludedDirectories(string rootDirectory,
                                                                IList<string> filesInDirectory,
                                                                IList<string> excludedDirectories,
                                                                bool isRecursive)
        {
            try
            {
                if (String.IsNullOrEmpty(rootDirectory))
                    return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "Root directory was not provided");

                if (filesInDirectory.IsNullOrEmpty())
                    return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "List of files in directory was not provided");

                if (!isRecursive || excludedDirectories.IsNullOrEmpty())
                    return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectory);

                foreach (var file in filesInDirectory)
                {
                    var substringStartIndex = rootDirectory.Length - 1;
                    var substringLength = file.LastIndexOf('\\') != -1
                        ? (file.LastIndexOf('\\') + 1) - rootDirectory.Length
                        : file.Length - rootDirectory.Length;

                    var relativePathToFile = file.Substring(substringStartIndex, substringLength);

                    foreach (var excludedDirectory in excludedDirectories)
                    {
                        if (!relativePathToFile.Contains(excludedDirectory))
                            continue;

                        filesInDirectory.Remove(file);
                    }
                }

                return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectory);
            }
            catch (Exception ex)
            {
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, ex);
            }
        }

        public FileDiscoveryResult FilterOutExcludedFileMasks(string rootDirectory,
                                                              IList<string> filesInDirectory,
                                                              IList<string> excludedFileMasks)
        {
            if (String.IsNullOrEmpty(rootDirectory))
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "Root directory was not provided");

            if (filesInDirectory.IsNullOrEmpty())
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "List of files in directory was not provided");

            if (excludedFileMasks.IsNullOrEmpty())
                return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectory);

            try
            {
                var filteredFilesHashSet = new HashSet<string>();
                foreach (var filePath in filesInDirectory)
                {
                    var filename = Path.GetFileName(filePath);
                    if (String.IsNullOrEmpty(filename))
                        continue;

                    foreach (var excludedFileMask in excludedFileMasks)
                    {
                        var regexPattern = excludedFileMask.ConvertWildcardPatternToRegexPattern();
                        if (Regex.IsMatch(filePath, regexPattern))
                            continue;
                        
                        filteredFilesHashSet.Add(filePath);
                    }
                }

                return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filteredFilesHashSet.ToList());
            }
            catch (Exception ex)
            {
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, ex);
            }
        }

    }
}
