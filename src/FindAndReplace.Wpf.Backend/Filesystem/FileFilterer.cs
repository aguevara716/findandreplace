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
        private readonly IRelativePathExtractor _relativePathExtractor;

        public FileFilterer(IRelativePathExtractor relativePathExtractor)
        {
            _relativePathExtractor = relativePathExtractor;
        }

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

                foreach (var file in filesInDirectory.ToList())
                {
                    var relativePath = _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, file);
                    foreach (var excludedDirectory in excludedDirectories)
                    {
                        if (!relativePath.Contains(excludedDirectory))
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

                foreach(var excludedFileMask in excludedFileMasks)
                {
                    var regexPattern = excludedFileMask.ConvertWildcardPatternToRegexPattern();

                    foreach(var filePath in filesInDirectory.ToList())
                    {
                        var filename = Path.GetFileName(filePath);
                        if (string.IsNullOrEmpty(filename))
                            continue;
                        if (Regex.IsMatch(filePath, regexPattern))
                            continue;

                        filteredFilesHashSet.Add(filePath);
                    }

                    filesInDirectory = filteredFilesHashSet.ToList();
                    filteredFilesHashSet.Clear();
                }

                return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, filesInDirectory);
            }
            catch (Exception ex)
            {
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, ex);
            }
        }

    }
}
