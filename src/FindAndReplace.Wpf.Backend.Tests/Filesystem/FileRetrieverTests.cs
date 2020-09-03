using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FindAndReplace.Wpf.Backend.Filesystem;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Filesystem
{
    [TestFixture]
    public class FileRetrieverTests
    {
        private IFileFilterer _fileFilterer;

        [SetUp]
        public void BeforeEach()
        {
            _fileFilterer = new FileFilterer();
        }

        //FileDiscoveryResult FilterOutExcludedDirectories(string rootDirectory,
        //                                                 IList<string> filesInDirectory,
        //                                                 IList<string> excludedDirectories,
        //                                                 bool isRecursive);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedDirectories_Should_ReturnFailureIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            string rootDirectory = isNull
                ? null
                : string.Empty;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory, null, null, false);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedDirectories_Should_ReturnFailureIfFilesInDirectoryIsNullOrEmpty(bool isNull)
        {
            IList<string> filesInDirectory = isNull
                ? null
                : Enumerable.Empty<string>().ToList();

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories("asdf", filesInDirectory, null, false);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FilterOutExcludedDirectories_Should_ReturnSuccessIfIsRecursiveIsFalse()
        {
            var rootDirectory = "asdf";
            var filesInDirectory = new List<string>
            {
                "asdf",
                "asdf"
            };
            var excludedDirectories = new List<string> 
            {
                "asdf"
            };
            var isRecursive = false;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory, 
                                                                                 filesInDirectory, 
                                                                                 excludedDirectories, 
                                                                                 isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.Should().BeEquivalentTo(filesInDirectory);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedDirectories_Should_ReturnSuccessIfExcludedDirectoriesIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = "asdf";
            var filesInDirectory = new List<string>
            {
                "asdf",
                "asdf"
            };
            var excludedDirectories = isNull
                ? null
                : Enumerable.Empty<string>().ToList();
            var isRecursive = true;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory,
                                                                                 filesInDirectory,
                                                                                 excludedDirectories,
                                                                                 isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.Should().BeEquivalentTo(filesInDirectory);
        }

        private IEnumerable<string> SeedFilesInDirectory(string rootPath, int count)
        {
            if (!rootPath.EndsWith('\\'))
                rootPath += '\\';

            var extensions = new List<string>
            {
                ".cs",
                ".exe",
                ".zip",
                ".pdf"
            };
            for (var index = 0; index <= count; index++)
            {
                var extension = extensions[index % extensions.Count];
                yield return $"{rootPath}File-{index}{extension}";
            }
        }

        [Test]
        public void FilterOutExcludedDirectories_Should_RemoveFilesWithinExcludedDirectories()
        {
            var rootDirectory = @"C:\Users\devnull\";
            var filesInDirectory = SeedFilesInDirectory(rootDirectory, 10)
                .Union(SeedFilesInDirectory($"{rootDirectory}ExcludeMe", 10))
                .Union(SeedFilesInDirectory($"{rootDirectory}IncludeMe", 10))
                .Union(SeedFilesInDirectory(@$"{rootDirectory}FirstDirectory\SecondDirectory\ExcludeMe", 10))
                .ToList();
            var excludedDirectories = new List<string>
            {
                "ExcludeMe"
            };
            var isRecursive = true;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory,
                                                                                 filesInDirectory,
                                                                                 excludedDirectories,
                                                                                 isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.All(f => !f.Contains("ExcludeMe"));
        }

        //FileDiscoveryResult FilterOutExcludedFileMasks(string rootDirectory,
        //                                               IList<string> filesInDirectory,
        //                                               IList<string> excludedFileMasks);
        [Test]
        public void FilterOutExcludedFileMasks_Should_()
        {

        }
    }
}
