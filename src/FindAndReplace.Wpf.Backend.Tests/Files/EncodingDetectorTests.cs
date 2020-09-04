using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Backend.Files;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class EncodingDetectorTests
    {
        // Dependencies
        private IEncodingDetector _encodingDetector;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _encodingDetector = new Backend.Files.EncodingDetector();
        }

        // Private Methods
        private string GetFile(string searchPattern)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var firstMatch = Directory.GetFiles(rootDirectory, searchPattern).First();
            return firstMatch;
        }

        private byte[] GetFileSample(string filePath)
        {
            var fileReader = new FileReader();
            var fileSampleResult = fileReader.GetFileSampleData(filePath);
            if (!fileSampleResult.IsSuccessful)
                throw new Exception($"Unable to get sample data for file \"{filePath}\"");
            return fileSampleResult.Sample;
        }

        // EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes);
        // EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes, Encoding fallbackEncoding);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DetectFileEncoding_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;
            var sampleData = new byte[2] { 0, 0 };

            var encodingResult = _encodingDetector.DetectFileEncoding(filePath, sampleData);

            encodingResult.IsSuccessful.Should().BeFalse();
            encodingResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DetectFileEncoding_Should_ReturnFailureIfSampleDataIsNullOrEmpty(bool isNull)
        {
            string filePath = GetFile("*.json");
            byte[] sampleData = isNull
                ? null
                : Enumerable.Empty<byte>().ToArray();

            var encodingResult = _encodingDetector.DetectFileEncoding(filePath, sampleData);

            encodingResult.IsSuccessful.Should().BeFalse();
            encodingResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void DetectFileEncoding_Should_ReturnEncodingForFile()
        {
            var filePath = GetFile("*.json");
            var fileSample = GetFileSample(filePath);

            var encodingDetectorResult = _encodingDetector.DetectFileEncoding(filePath, fileSample);

            encodingDetectorResult.IsSuccessful.Should().BeTrue();
            encodingDetectorResult.FileEncoding.EncodingName.Should().Be(Encoding.UTF8.EncodingName, because: $"expected \"{filePath}\" to be the given encoding");
        }
    }
}
