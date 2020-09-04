using System;
using System.IO;
using System.Linq;
using FindAndReplace.Wpf.Backend.Files;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class BinaryFileDetectorTests
    {
        // Dependencies
        private IBinaryFileDetector _binaryFileDetector;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _binaryFileDetector = new BinaryFileDetector();
        }

        // Private Methods
        private string GetFilePath()
        {
            return GetFilePath("*.dll");
        }

        private string GetFilePath(string searchPattern)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var firstDll = Directory.GetFiles(rootDirectory, searchPattern, SearchOption.AllDirectories).First();
            return firstDll;
        }

        private byte[] GetFileSample(string filePath)
        {
            var fileReader = new FileReader();
            var fileSampleResult = fileReader.GetFileSampleData(filePath);
            if (!fileSampleResult.IsSuccessful)
                throw new Exception($"Unable to get sample for {filePath}");
            return fileSampleResult.Sample;
        }

        // BinaryFileDetectionResult CheckIsBinaryFile(string filePath, byte[] sampleBytes);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CheckIsBinaryFile_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;
            var sampleBytes = new byte[2] { 0, 0 };

            var binaryDetectionResult = _binaryFileDetector.CheckIsBinaryFile(filePath, sampleBytes);

            binaryDetectionResult.IsSuccessful.Should().BeFalse();
            binaryDetectionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CheckIsBinaryFile_Should_ReturnFailureIfSampleBytesIsNullOrEmpty(bool isNull)
        {
            var filePath = GetFilePath();
            byte[] sampleBytes = isNull
                ? null
                : Enumerable.Empty<byte>().ToArray();

            var binaryDetectionResult = _binaryFileDetector.CheckIsBinaryFile(filePath, sampleBytes);

            binaryDetectionResult.IsSuccessful.Should().BeFalse();
            binaryDetectionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        [TestCase("*.dll")]
        [TestCase("*.exe")]
        public void CheckIsBinaryFile_Should_ReturnTrueIfFileIsBinary(string searchPattern)
        {
            var filePath = GetFilePath(searchPattern);
            var sampleBytes = GetFileSample(filePath);

            var binaryDetectionResult = _binaryFileDetector.CheckIsBinaryFile(filePath, sampleBytes);

            binaryDetectionResult.IsSuccessful.Should().BeTrue();
            binaryDetectionResult.IsBinaryFile.Should().BeTrue();
        }

        [Test]
        [TestCase("*.json")]
        [TestCase("*.tmp")]
        public void CheckIsBinaryFile_Should_ReturnFalseIfFileIsNotBinary(string searchPattern)
        {
            var filePath = GetFilePath(searchPattern);
            var sampleBytes = GetFileSample(filePath);

            var binaryDetectionResult = _binaryFileDetector.CheckIsBinaryFile(filePath, sampleBytes);

            binaryDetectionResult.IsSuccessful.Should().BeTrue();
            binaryDetectionResult.IsBinaryFile.Should().BeFalse();
        }

    }
}
