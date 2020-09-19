using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Parameters;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class FileWriterTests
    {
        // Constants
        private const int SLEEP_TIME = 1500;
        private const string TEST_FILE_NAME = "test-file.txt";

        // Dependencies
        private IFileWriter _fileWriter;

        // Initialization
        [OneTimeSetUp]
        public void BeforeAny()
        {
            if (File.Exists(TEST_FILE_NAME))
            {
                File.Delete(TEST_FILE_NAME);
                Thread.Sleep(SLEEP_TIME);
            }

            File.Create(TEST_FILE_NAME);
        }

        [SetUp]
        public void BeforeEach()
        {
            _fileWriter = new FileWriter();
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            if (!File.Exists(TEST_FILE_NAME))
                return;

            File.Delete(TEST_FILE_NAME);
        }

        // Private Methods
        private async Task<string> GetFileContentsAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Unable to find file \"{filePath}\"");

            using var fr = new StreamReader(filePath);
            var content = await fr.ReadToEndAsync();
            return content;
        }

        // Task<FileWriterResult> WriteTextAsync(FileWriterParameters fileWriterParameters);
        [Test]
        public async Task WriteTextAsync_Should_ReturnFailureIfParametersAreNull()
        {
            FileWriterParameters fileWriterParameters = null;

            var fileWriterResult = await _fileWriter.WriteTextAsync(fileWriterParameters);

            fileWriterResult.ErrorMessage.Should().NotBeNullOrEmpty();
            fileWriterResult.Exception.Should().BeNull();
            fileWriterResult.IsFileUpdated.Should().BeFalse();
            fileWriterResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task WriteTextAsync_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            var fileWriterParameters = new FileWriterParameters
            {
                FilePath = isNull
                    ? null
                    : string.Empty,
                IsKeepingOriginalModificationDate = false,
                NewFileContent = Guid.NewGuid().ToString()
            };

            var fileWriterResult = await _fileWriter.WriteTextAsync(fileWriterParameters);

            fileWriterResult.ErrorMessage.Should().NotBeNullOrEmpty();
            fileWriterResult.Exception.Should().BeNull();
            fileWriterResult.IsFileUpdated.Should().BeFalse();
            fileWriterResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        public async Task WriteTextAsync_Should_ReturnFailureIfFileContentIsNull()
        {
            var fileWriterParameters = new FileWriterParameters
            {
                FilePath = Guid.NewGuid().ToString(),
                IsKeepingOriginalModificationDate = false,
                NewFileContent = null
            };

            var fileWriterResult = await _fileWriter.WriteTextAsync(fileWriterParameters);

            fileWriterResult.ErrorMessage.Should().NotBeNullOrEmpty();
            fileWriterResult.Exception.Should().BeNull();
            fileWriterResult.IsFileUpdated.Should().BeFalse();
            fileWriterResult.IsSuccessful.Should().BeFalse();
        }

        [Test]
        public async Task WriteTextAsync_Should_OverwriteAllTextInFile()
        {
            Thread.Sleep(SLEEP_TIME);
            var fileWriterParameters = new FileWriterParameters
            {
                FilePath = TEST_FILE_NAME,
                IsKeepingOriginalModificationDate = false,
                NewFileContent = Guid.NewGuid().ToString()
            };

            var fileWriterResult = await _fileWriter.WriteTextAsync(fileWriterParameters);

            fileWriterResult.ErrorMessage.Should().BeNullOrEmpty();
            fileWriterResult.Exception.Should().BeNull();
            fileWriterResult.IsFileUpdated.Should().BeTrue();
            fileWriterResult.IsSuccessful.Should().BeTrue();
            var newContent = await GetFileContentsAsync(fileWriterParameters.FilePath);
            newContent.Should().Be(fileWriterParameters.NewFileContent);
        }

    }
}
