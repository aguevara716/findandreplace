using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FindAndReplace.Wpf.Backend.Filesystem;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Filesystem
{
    [TestFixture]
    public class FileFiltererTests
    {
        private IFileFilterer _fileFilterer;

        [SetUp]
        public void BeforeEach()
        {
            _fileFilterer = new FileFilterer();
        }

        // Private Methods
        private IEnumerable<string> SeedFilesInDirectory(string rootPath, int count)
        {
            if (!rootPath.EndsWith(Path.DirectorySeparatorChar))
                rootPath += Path.DirectorySeparatorChar;

            var extensions = new List<string>
            {
                ".cs",
                ".exe",
                ".zip",
                ".pdf"
            };
            for (var index = 0; index < count; index++)
            {
                var extension = extensions[index % extensions.Count];
                yield return $"{rootPath}File-{index}{extension}";
            }
        }


        //FileDiscoveryResult FilterOutExcludedDirectories(string rootDirectory, IList<string> filesInDirectory, IList<string> excludedDirectories, bool isRecursive);
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

        [Test]
        public void FilterOutExcludedDirectories_Should_RemoveFilesWithinExcludedDirectories()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filesInDirectory = SeedFilesInDirectory(rootDirectory, 10)
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}ExcludeMe", 10))
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}ExcludeMe2", 10))
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}ExcludeMe3", 10))
                .Union(SeedFilesInDirectory(@$"{rootDirectory}{Path.DirectorySeparatorChar}FirstDirectory{Path.DirectorySeparatorChar}SecondDirectory{Path.DirectorySeparatorChar}ExcludeMe4", 10))
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}IncludeMe", 10))
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}Exclusion", 10))
                .ToList();
            var excludedDirectories = new List<string>
            {
                "ExcludeMe",
                "Exclusion"
            };
            var isRecursive = true;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory,
                                                                                 filesInDirectory,
                                                                                 excludedDirectories,
                                                                                 isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            foreach (var excludedDirectory in excludedDirectories)
            {
                fileDiscoveryResult.Files.All(f => !f.Contains(excludedDirectory)).Should().BeTrue();
            }
        }

        [Test]
        public void FilterOutExcludedDirectories_Should_ReturnFailureIfExceptionWasThrown()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filesInDirectory = SeedFilesInDirectory(rootDirectory, 10)
                // Without the root directory here, it'll trigger an out of range exeption
                .Union(SeedFilesInDirectory($"{Path.DirectorySeparatorChar}Folder", 10))
                .ToList();
            var excludedDirectories = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
            var isRecursive = true;

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedDirectories(rootDirectory,
                                                                                 filesInDirectory,
                                                                                 excludedDirectories,
                                                                                 isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
            fileDiscoveryResult.Exception.Should().NotBeNull().And.BeOfType(typeof(ArgumentOutOfRangeException));
        }

        //FileDiscoveryResult FilterOutExcludedFileMasks(string rootDirectory, IList<string> filesInDirectory, IList<string> excludedFileMasks);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedFileMasks_Should_ReturnFailureIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = isNull
                ? null
                : string.Empty;
            var filesInDirectory = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
            var excludedFileMasks = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedFileMasks_Should_ReturnFailureIfFilesInDirectoryIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            IList<string> filesInDirectory = isNull
                ? null
                : Enumerable.Empty<string>().ToList();
            var excludedFileMasks = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FilterOutExcludedFileMasks_Should_ReturnSuccessIfExcludedFileMasksIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filesInDirectory = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
            IList<string> excludedFileMasks = isNull
                ? null
                : Enumerable.Empty<string>().ToList();

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Path.Should().Be(rootDirectory);
            fileDiscoveryResult.Files.Should().BeEquivalentTo(filesInDirectory);
        }

        [Test]
        public void FilterOutExcludedFileMasks_Should_RemoveFilesWithExcludedFilenames()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filesInDirectory = SeedFilesInDirectory(rootDirectory, 10)
                .Union(SeedFilesInDirectory($"{rootDirectory}{Path.DirectorySeparatorChar}FirstDirectory", 10))
                .Union(SeedFilesInDirectory(@$"{rootDirectory}{Path.DirectorySeparatorChar}FirstDirectory{Path.DirectorySeparatorChar}SecondDirectory", 10))
                .ToList();
            var excludedFileMasks = new List<string>()
            {
                "*7*", "*8*", "*9*"
            };

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.Should().NotHaveSameCount(filesInDirectory);
            foreach(var excludedFileMask in excludedFileMasks)
            {
                fileDiscoveryResult.Files.All(f => !f.Contains(excludedFileMask)).Should().BeTrue();
            }
        }

        [Test]
        public void FilterOutExcludedFileMasks_Should_ReturnFailureIfExceptionWasThrown()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filesInDirectory = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
            var excludedFileMasks = Enumerable.Range(0, 10).Select(x => null as string).ToList();

            var fileDiscoveryResult = _fileFilterer.FilterOutExcludedFileMasks(rootDirectory, filesInDirectory, excludedFileMasks);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
            fileDiscoveryResult.Exception.Should().NotBeNull();
        }

    }
}
