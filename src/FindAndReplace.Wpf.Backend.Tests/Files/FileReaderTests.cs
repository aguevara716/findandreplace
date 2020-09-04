using System;
using System.IO;
using System.Linq;
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
        private string GetFilePath()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var firstDll = Directory.GetFiles(rootDirectory, "*.dll").First();
            return firstDll;
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
            var filePath = GetFilePath();

            var fileSampleResult = _fileReader.GetFileSampleData(filePath);

            fileSampleResult.IsSuccessful.Should().BeTrue();
            fileSampleResult.Sample.Should().NotBeNullOrEmpty();
        }
    }
}
