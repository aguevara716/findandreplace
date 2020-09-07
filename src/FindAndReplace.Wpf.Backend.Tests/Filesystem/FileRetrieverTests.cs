using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.Backend.Tests.Files;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Filesystem
{
    [TestFixture]
    public class FileRetrieverTests
    {
        // Constants
        private readonly string ROOT_DIRECTORY = Path.Combine(Directory.GetCurrentDirectory(), "SampleFiles");
        private const string INNER_DIRECTORY_NAME = "InnerDirectory";
        
        // Dependencies
        private IFileRetriever _fileRetriever;

        // Initialization
        [OneTimeSetUp]
        public void BeforeAny()
        {
            DeleteDirectoryIfExists(ROOT_DIRECTORY);

            PopulateDirectory(10, ROOT_DIRECTORY);
            PopulateDirectory(10, ROOT_DIRECTORY, INNER_DIRECTORY_NAME);
        }

        [SetUp]
        public void BeforeEach()
        {
            _fileRetriever = new FileRetriever();
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            DeleteDirectoryIfExists(ROOT_DIRECTORY);
        }

        // Private Methods
        private void DeleteDirectoryIfExists(string path)
        {
            if (!Directory.Exists(path))
                return;
            Directory.Delete(path, true);
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (Directory.Exists(path))
                return;
            Directory.CreateDirectory(path);
        }

        private void PopulateDirectory(int fileQuantity, params string[] paths)
        {
            var path = Path.Combine(paths);
            CreateDirectoryIfNotExists(path);

            var filepaths = FilePathGenerator.GenerateFilePaths(path, fileQuantity);
            FilePathGenerator.GenerateFiles(filepaths);
        }

        // FileDiscoveryResult GetFiles(string rootDirectory, IList<string> fileMasks, bool isRecursive)
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetFiles_Should_ReturnFailureIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            string rootDirectory = isNull
                ? null
                : string.Empty;
            var fileMasks = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();
            var isRecursive = false;
            
            var fileDiscoveryResult = _fileRetriever.GetFiles(rootDirectory, fileMasks, isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
            fileDiscoveryResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GetFiles_Should_ReturnAllFilesIfFileMaskNotSet()
        {
            var rootDirectory = ROOT_DIRECTORY;
            var fileMasks = new List<string>();
            var isRecursive = false;

            var fileDiscoveryResult = _fileRetriever.GetFiles(rootDirectory, fileMasks, isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.Should().BeEquivalentTo(Directory.GetFiles(rootDirectory));
        }

        [Test]
        public void GetFiles_Should_ReturnFilesMatchingFileMask()
        {
            var rootDirectory = ROOT_DIRECTORY;
            var fileMasks = new List<string>
            {
                "*.cs",
                "*.txt"
            };
            var isRecursive = false;

            var fileDiscoveryResult = _fileRetriever.GetFiles(rootDirectory, fileMasks, isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            var extensions = fileDiscoveryResult.Files.Select(f => $"*{Path.GetExtension(f)}").Distinct();
            extensions.Should().BeEquivalentTo(fileMasks);
        }

        [Test]
        public void GetFiles_Should_RetrieveFilesRecursivelyIfIsRecursiveIsTrue()
        {
            var rootDirectory = ROOT_DIRECTORY;
            var fileMasks = new List<string> { "*.cs" };
            var isRecursive = true;

            var fileDiscoveryResult = _fileRetriever.GetFiles(rootDirectory, fileMasks, isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeTrue();
            fileDiscoveryResult.Files.Any(f => f.Contains(INNER_DIRECTORY_NAME)).Should().BeTrue();
        }

        [Test]
        public void GetFiles_Should_ReturnFailureIfAnExceptionIsThrown()
        {
            var fakeRootDirectory = Path.Combine(ROOT_DIRECTORY, Guid.NewGuid().ToString());
            var fileMasks = new List<string> { "*.cs" };
            var isRecursive = true;

            var fileDiscoveryResult = _fileRetriever.GetFiles(fakeRootDirectory, fileMasks, isRecursive);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
            fileDiscoveryResult.ErrorMessage.Should().NotBeNullOrEmpty();
            fileDiscoveryResult.Exception.Should().NotBeNull().And.BeOfType<DirectoryNotFoundException>();
        }

    }
}
