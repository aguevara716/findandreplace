using System;
using System.Collections.Generic;
using System.Linq;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.Backend.Results;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Filesystem
{
    [TestFixture]
    public class FileDiscovererTests
    {
        private IFileDiscoverer _fileDiscoverer;
        private IFileFilterer _fileFilterer;
        private IFileRetriever _fileRetriever;

        [SetUp]
        public void BeforeEach()
        {
            _fileFilterer = Substitute.For<IFileFilterer>();
            _fileRetriever = Substitute.For<IFileRetriever>();

            _fileDiscoverer = new FileDiscoverer(_fileFilterer, _fileRetriever);
        }

        // Private Methods
        private FileDiscoveryResult MockSuccess(string rootDirectory, int fileQuantity)
        {
            var files = Enumerable.Range(0, fileQuantity).Select(x => x.ToString()).ToList();
            return FileDiscoveryResult.CreateSuccess<FileDiscoveryResult>(rootDirectory, files);
        }

        private FileDiscoveryResult MockFailure(string rootDirectory, string errorMessage)
        {
            return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, errorMessage);
        }

        private FileDiscoveryResult MockFailure(string rootDirectory, Exception exception)
        {
            return FileDiscoveryResult.CreateFailure<FileDiscoveryResult>(rootDirectory, exception);
        }

        // FileDiscoveryResult DiscoverFiles(string rootDirectory, IList<string> fileMasks, IList<string> excludedDirectories, IList<string> excludedFileMasks, bool isRecursive)
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DiscoverFiles_Should_ReturnFailureIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            string rootDirectory = isNull
                ? null
                : string.Empty;

            var fileDiscoveryResult = _fileDiscoverer.DiscoverFiles(rootDirectory, null, null, null, false);

            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
            _fileRetriever.DidNotReceiveWithAnyArgs().GetFiles(null, null, false);
            _fileFilterer.DidNotReceiveWithAnyArgs().FilterOutExcludedDirectories(null, null, null, false);
            _fileFilterer.DidNotReceiveWithAnyArgs().FilterOutExcludedFileMasks(null, null, null);
        }

        // file retriever get files
        [Test]
        public void DiscoverFiles_Should_CallFileRetriever()
        {
            var rootDirectory = "asdf";
            var fileMasks = new List<string> { "*.cs" };
            var excludedDirectories = new List<string> { "Excluded" };
            var excludedFileMasks = new List<string> { "*excluded*" };
            var isRecursive = true;
            var failureResult = MockFailure(rootDirectory, "test");
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(failureResult);

            _ = _fileDiscoverer.DiscoverFiles(rootDirectory, fileMasks, excludedDirectories, excludedFileMasks, isRecursive);

            _fileRetriever.Received().GetFiles(rootDirectory, fileMasks, isRecursive);
        }

        [Test]
        public void DiscoverFiles_Should_ReturnFailureIfFileRetrieverFails()
        {
            var failureResult = MockFailure("asdf", "asdf");
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(failureResult);

            var fileDiscoveryResult = _fileDiscoverer.DiscoverFiles("asdf",
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    false);

            fileDiscoveryResult.Should().Be(failureResult);
        }

        [Test]
        public void DiscoverFiles_Should_ReturnFailureIfFileRetrieverReturnsZeroFiles()
        {
            var rootDirectory = "asdf";
            var fdr = MockSuccess(rootDirectory, 0);
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(fdr);

            var fileDiscoveryResult = _fileDiscoverer.DiscoverFiles(rootDirectory,
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    Enumerable.Empty<string>().ToList(),
                                                                    false);

            fileDiscoveryResult.ErrorMessage.Should().NotBeNullOrEmpty();
            fileDiscoveryResult.Exception.Should().BeNull();
            fileDiscoveryResult.Files.Should().BeNullOrEmpty();
            fileDiscoveryResult.IsSuccessful.Should().BeFalse();
        }

        // file filterer filter out excluded directories
        [Test]
        public void DiscoverFiles_Should_CallFilterOutExcludedDirectories()
        {
            var rootDirectory = "asdf";
            var fileMasks = new List<string> { "*.cs" };
            var excludedDirectories = new List<string> { "Excluded" };
            var excludedFileMasks = new List<string> { "*excluded*" };
            var isRecursive = true;
            var successResult = MockSuccess(rootDirectory, 10);
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(successResult);
            _fileFilterer.FilterOutExcludedDirectories(null, null, null, false).ReturnsForAnyArgs(successResult);

            _ = _fileDiscoverer.DiscoverFiles(rootDirectory, fileMasks, excludedDirectories, excludedFileMasks, isRecursive);

            _fileFilterer.Received().FilterOutExcludedDirectories(rootDirectory, successResult.Files, excludedDirectories, isRecursive);
        }

        [Test]
        public void DiscoverFiles_Should_ReturnFailureIfFilterOutExcludedDirectoriesFails()
        {
            var rootDirectory = "asdf";
            var fileMasks = new List<string> { "*.cs" };
            var excludedDirectories = new List<string> { "Excluded" };
            var excludedFileMasks = new List<string> { "*excluded*" };
            var isRecursive = true;
            var successResult = MockSuccess(rootDirectory, 10);
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(successResult);
            var failureResult = MockFailure(rootDirectory, "test");
            _fileFilterer.FilterOutExcludedDirectories(null, null, null, false).ReturnsForAnyArgs(failureResult);

            var fileDiscoveryResult = _fileDiscoverer.DiscoverFiles(rootDirectory, fileMasks, excludedDirectories, excludedFileMasks, isRecursive);

            fileDiscoveryResult.Should().Be(failureResult);
        }

        // file filterer filter out excluded file masks
        [Test]
        public void DiscoverFiles_Should_CallFilterOutExcludedFileMasks()
        {
            var rootDirectory = "asdf";
            var fileMasks = new List<string> { "*.cs" };
            var excludedDirectories = new List<string> { "Excluded" };
            var excludedFileMasks = new List<string> { "*excluded*" };
            var isRecursive = true;
            var successResult = MockSuccess("asdf", 10);
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(successResult);
            _fileFilterer.FilterOutExcludedDirectories(null, null, null, false).ReturnsForAnyArgs(successResult);

            _ = _fileDiscoverer.DiscoverFiles(rootDirectory, fileMasks, excludedDirectories, excludedFileMasks, isRecursive);

            _fileFilterer.Received().FilterOutExcludedFileMasks(rootDirectory, successResult.Files, excludedFileMasks);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DiscoverFiles_Should_ReturnResultFromFilterOutExcludedFileMasks(bool isFilterOutExcludedFileMasksSuccessful)
        {
            var rootDirectory = "asdf";
            var fileMasks = new List<string> { "*.cs" };
            var excludedDirectories = new List<string> { "Excluded" };
            var excludedFileMasks = new List<string> { "*excluded*" };
            var isRecursive = true;
            var successResult = MockSuccess(rootDirectory, 10);
            _fileRetriever.GetFiles(null, null, false).ReturnsForAnyArgs(successResult);
            _fileFilterer.FilterOutExcludedDirectories(null, null, null, false).ReturnsForAnyArgs(successResult);
            var filterOutExcludedFileMasksResult = isFilterOutExcludedFileMasksSuccessful
                ? MockSuccess(rootDirectory, 5)
                : MockFailure(rootDirectory, "test");
            _fileFilterer.FilterOutExcludedFileMasks(null, null, null).ReturnsForAnyArgs(filterOutExcludedFileMasksResult);


            var fileDiscoveryResult = _fileDiscoverer.DiscoverFiles(rootDirectory, fileMasks, excludedDirectories, excludedFileMasks, isRecursive);

            fileDiscoveryResult.Should().Be(filterOutExcludedFileMasksResult);
        }

    }
}
