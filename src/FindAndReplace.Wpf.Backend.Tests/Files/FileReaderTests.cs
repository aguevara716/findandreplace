using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Files;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class FileReaderTests
    {
        // Dependencies
        private IFileReader _fileReader;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _fileReader = new FileReader();
        }

        // Private Methods
        private string GetFilePath(string searchCriteria)
        {
            var firstDll = FilePathGenerator.GetRealFiles(Directory.GetCurrentDirectory(), searchCriteria)
                                            .First();
            return firstDll;
        }

        // FileContentResult GetFileContent(string filePath);
        // Task<FileContentResult> GetFileContentAsync(string filePath);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetFileContentAsync_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;

            var fileContentResult = await _fileReader.GetFileContentAsync(filePath);

            fileContentResult.IsSuccessful.Should().BeFalse();
            fileContentResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task GetFileContentAsync_Should_ReturnSuccessForTextFiles()
        {
            var filePath = GetFilePath("*.json");

            var fileContentResult = await _fileReader.GetFileContentAsync(filePath);

            fileContentResult.IsSuccessful.Should().BeTrue();
            fileContentResult.Content.Should().NotBeNullOrEmpty();
        }

        // FileSampleResult GetFileSampleData(string filePath);
        // FileSampleResult GetFileSampleData(string filePath, int numberOfBytesToIngest);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetFileSampleData_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;

            var fileSampleResult = _fileReader.GetFileSampleData(filePath);

            fileSampleResult.IsSuccessful.Should().BeFalse();
            fileSampleResult.Exception.Should().NotBeNull();
            fileSampleResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GetFileSampleData_Should_ReturnFileSampleData()
        {
            var filePath = GetFilePath("*.dll");

            var fileSampleResult = _fileReader.GetFileSampleData(filePath);

            fileSampleResult.IsSuccessful.Should().BeTrue();
            fileSampleResult.Sample.Should().NotBeNullOrEmpty();
        }
    }
}
