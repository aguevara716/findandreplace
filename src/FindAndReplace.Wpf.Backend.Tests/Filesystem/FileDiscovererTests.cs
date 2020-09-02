using FindAndReplace.Wpf.Backend.Filesystem;
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

        // FileDiscoveryResult DiscoverFiles(string rootDirectory, 
        //    IList<string> fileMasks, 
        //    IList<string> excludedDirectories, 
        //    IList<string> excludedFileMasks, 
        //    bool isRecursive)
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

    }
}
