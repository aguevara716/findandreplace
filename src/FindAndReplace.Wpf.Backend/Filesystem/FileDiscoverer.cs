﻿using System;
using System.Collections.Generic;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Filesystem
{
    public interface IFileDiscoverer
    {
        FileDiscoveryResult DiscoverFiles(string rootDirectory,
                                          IList<string> fileMasks,
                                          IList<string> excludedDirectories,
                                          IList<string> excludedFileMasks,
                                          bool isRecursive);
    }

    public class FileDiscoverer : IFileDiscoverer
    {
        private readonly IFileFilterer _fileFilterer;
        private readonly IFileRetriever _fileRetriever;

        public FileDiscoverer(IFileFilterer fileFilterer, IFileRetriever fileRetriever)
        {
            _fileFilterer = fileFilterer;
            _fileRetriever = fileRetriever;
        }

        public FileDiscoveryResult DiscoverFiles(string rootDirectory, 
                                                 IList<string> fileMasks, 
                                                 IList<string> excludedDirectories, 
                                                 IList<string> excludedFileMasks,
                                                 bool isRecursive)
        {
            if (String.IsNullOrEmpty(rootDirectory))
                return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, "Root directory is required");

            var filesMatchingMasksResult = _fileRetriever.GetFiles(rootDirectory, fileMasks, isRecursive);
            if (!filesMatchingMasksResult.IsSuccessful)
                return filesMatchingMasksResult;

            var filesInDirectory = filesMatchingMasksResult.Files;

            var filesFilteredByExcludedDirectoriesResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory, filesInDirectory, excludedDirectories, isRecursive);
            if (!filesFilteredByExcludedDirectoriesResult.IsSuccessful)
                return filesFilteredByExcludedDirectoriesResult;

            var filesFilteredByExcludedFileMasksResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);
            return filesFilteredByExcludedFileMasksResult;
        }
    }
}
