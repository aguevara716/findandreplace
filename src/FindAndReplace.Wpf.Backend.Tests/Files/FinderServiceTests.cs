using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Results;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class FinderServiceTests
    {
        // Dependencies
        private IBinaryFileDetector _binaryFileDetector;
        private IFileReader _fileReader;
        private IMatchPreviewExtractor _matchPreviewExtractor;
        private ITextMatcher _textMatcher;
        private IFinderService _finderService;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _binaryFileDetector = Substitute.For<IBinaryFileDetector>();
            _fileReader = Substitute.For<IFileReader>();
            _matchPreviewExtractor = Substitute.For<IMatchPreviewExtractor>();
            _textMatcher = Substitute.For<ITextMatcher>();

            _finderService = new FinderService(_binaryFileDetector,
                                               _fileReader,
                                               _matchPreviewExtractor,
                                               _textMatcher);
        }

        // Task<TextMatcherResult> FindTextInFileAsync(string filePath, string findText, bool isRegexSearch, bool isUsingEscapeCharacters, bool isCaseSensitive);
        [Test]
        public async Task FindTextInFileAsync_Should_CallFileReader()
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            _fileReader.GetFileSampleData(filePath).Returns(FileSampleResult.CreateFailure<FileSampleResult>(filePath, Guid.NewGuid().ToString(), new Exception(Guid.NewGuid().ToString())));

            _ = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            _fileReader.Received().GetFileSampleData(filePath);
        }

        [Test]
        public async Task FindTextInFileAsync_Should_ReturnIfGetFileSampleDataFails()
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            var fileSampleResultFailure = FileSampleResult.CreateFailure<FileSampleResult>(filePath, Guid.NewGuid().ToString(), new Exception(Guid.NewGuid().ToString()));
            _fileReader.GetFileSampleData(filePath).Returns(fileSampleResultFailure);

            var textMatcherResult = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().Be(fileSampleResultFailure.ErrorMessage);
            textMatcherResult.Exception.Should().Be(fileSampleResultFailure.Exception);
            _fileReader.Received().GetFileSampleData(filePath);
            _binaryFileDetector.DidNotReceiveWithAnyArgs().CheckIsBinaryFile(null, null);
            _ = _fileReader.DidNotReceiveWithAnyArgs().GetFileContentAsync(null);
            _textMatcher.DidNotReceiveWithAnyArgs().FindTextInFile(null, null, null, false, false, false);
        }

        [Test]
        public async Task FindTextInFileAsync_Should_ReturnIfBinaryFileDetectionFails()
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            var sampleData = new byte[] { 123, 123, 123 };
            _fileReader.GetFileSampleData(filePath).Returns(FileSampleResult.CreateSuccess<FileSampleResult>(filePath, sampleData));
            var binaryDetectionResultFailure = BinaryFileDetectionResult.CreateFailure<BinaryFileDetectionResult>(filePath, Guid.NewGuid().ToString(), new Exception(Guid.NewGuid().ToString()));
            _binaryFileDetector.CheckIsBinaryFile(filePath, Arg.Any<byte[]>()).Returns(binaryDetectionResultFailure);

            var textMatcherResult = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().Be(binaryDetectionResultFailure.ErrorMessage);
            textMatcherResult.Exception.Should().Be(binaryDetectionResultFailure.Exception);
            _fileReader.Received().GetFileSampleData(filePath);
            _binaryFileDetector.Received().CheckIsBinaryFile(filePath, sampleData);
            _ = _fileReader.DidNotReceiveWithAnyArgs().GetFileContentAsync(null);
            _textMatcher.DidNotReceiveWithAnyArgs().FindTextInFile(null, null, null, false, false, false);
        }

        [Test]
        public async Task FindTextInFileAsync_Should_ReturnFailureIfBinaryFileDetectionReturnsTrue()
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            var sampleData = new byte[] { 123, 123, 123 };
            _fileReader.GetFileSampleData(filePath).Returns(FileSampleResult.CreateSuccess<FileSampleResult>(filePath, sampleData));
            var binaryDetectionResultSuccess = BinaryFileDetectionResult.CreateSuccess<BinaryFileDetectionResult>(filePath, true);
            _binaryFileDetector.CheckIsBinaryFile(filePath, Arg.Any<byte[]>()).Returns(binaryDetectionResultSuccess);

            var textMatcherResult = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textMatcherResult.Exception.Should().BeNull();
            _fileReader.Received().GetFileSampleData(filePath);
            _binaryFileDetector.Received().CheckIsBinaryFile(filePath, sampleData);
            _ = _fileReader.DidNotReceiveWithAnyArgs().GetFileContentAsync(null);
            _textMatcher.DidNotReceiveWithAnyArgs().FindTextInFile(null, null, null, false, false, false);
        }

        [Test]
        public async Task FindTextInFileAsync_Should_ReturnFailureIfFileContentRetrievalFails()
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            _fileReader.GetFileSampleData(filePath).ReturnsForAnyArgs(FileSampleResult.CreateSuccess<FileSampleResult>(filePath, null));
            _binaryFileDetector.CheckIsBinaryFile(filePath, Arg.Any<byte[]>()).ReturnsForAnyArgs(BinaryFileDetectionResult.CreateSuccess<BinaryFileDetectionResult>(filePath, false));
            var fileContentResultFailure = FileContentResult.CreateFailure<FileContentResult>(filePath, Guid.NewGuid().ToString(), new Exception(Guid.NewGuid().ToString()));
            _fileReader.GetFileContentAsync(filePath).Returns(fileContentResultFailure);

            var textMatcherResult = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().Be(fileContentResultFailure.ErrorMessage);
            textMatcherResult.Exception.Should().Be(fileContentResultFailure.Exception);
            _fileReader.Received().GetFileSampleData(filePath);
            _binaryFileDetector.Received().CheckIsBinaryFile(filePath, Arg.Any<byte[]>());
            _ = _fileReader.Received().GetFileContentAsync(filePath);
            _textMatcher.DidNotReceiveWithAnyArgs().FindTextInFile(null, null, null, false, false, false);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task FindTextInFile_Should_ReturnResultValueFromTextMatcher(bool isSuccessful)
        {
            var filePath = Guid.NewGuid().ToString();
            var findText = Guid.NewGuid().ToString();
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            _fileReader.GetFileSampleData(filePath).ReturnsForAnyArgs(FileSampleResult.CreateSuccess<FileSampleResult>(filePath, null));
            _binaryFileDetector.CheckIsBinaryFile(filePath, Arg.Any<byte[]>()).ReturnsForAnyArgs(BinaryFileDetectionResult.CreateSuccess<BinaryFileDetectionResult>(filePath, false));
            var fileContent = Guid.NewGuid().ToString();
            _fileReader.GetFileContentAsync(filePath).Returns(FileContentResult.CreateSuccess<FileContentResult>(filePath, fileContent));
            TextMatcherResult actualTextMatcherResult;
            if (isSuccessful)
            {
                var textMatches = new List<TextMatch>
                {
                    new TextMatch{StartIndex = 123, Length = 100 }
                };
                actualTextMatcherResult = TextMatcherResult.CreateSuccess<TextMatcherResult>(filePath, textMatches);
            }
            else
            {
                actualTextMatcherResult = TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, Guid.NewGuid().ToString(), new Exception(Guid.NewGuid().ToString()));
            }
            _textMatcher.FindTextInFile(filePath, findText, fileContent, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive).Returns(actualTextMatcherResult);

            var receivedTextMatcherResult = await _finderService.FindTextInFileAsync(filePath, findText, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            receivedTextMatcherResult.IsSuccessful.Should().Be(actualTextMatcherResult.IsSuccessful);
            receivedTextMatcherResult.ErrorMessage.Should().Be(actualTextMatcherResult.ErrorMessage);
            receivedTextMatcherResult.Exception.Should().Be(actualTextMatcherResult.Exception);
            receivedTextMatcherResult.Previews.Should().BeEquivalentTo(actualTextMatcherResult.TextMatches);
            _fileReader.Received().GetFileSampleData(filePath);
            _binaryFileDetector.Received().CheckIsBinaryFile(filePath, Arg.Any<byte[]>());
            _ = _fileReader.Received().GetFileContentAsync(filePath);
            _textMatcher.Received().FindTextInFile(filePath, findText, fileContent, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);
        }

        // IEnumerable<TextMatcherResult> FindTextInFiles(IDictionary<string, string> fileContentDictionary, bool isRegexSearch, bool isUsingEscapeCharacters, bool isCaseSensitive);
        [Test]
        public void FindTextInFiles_Should_LoopThroughAllFiles()
        {
            var fileContentDictionary = FilePathGenerator.GenerateFilePaths(100)
                                                         .AsParallel()
                                                         .ToDictionary(s => s, s => "asdf");
            var isRegexSearch = true;
            var isUsingEscapeCharacters = true;
            var isCaseSensitive = true;
            _fileReader.GetFileSampleData(Arg.Any<string>()).Returns(FileSampleResult.CreateFailure<FileSampleResult>("asdf", "asdf"));

            var textMatcherResult = _finderService.FindTextInFiles(fileContentDictionary, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            foreach(var fileContentKvp in fileContentDictionary)
            {
                _fileReader.Received().GetFileSampleData(fileContentKvp.Key);
            }
        }

    }
}
