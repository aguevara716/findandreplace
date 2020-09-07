using System;
using System.IO;
using FindAndReplace.Wpf.Backend.Filesystem;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Filesystem
{
    [TestFixture]
    public class RelativePathExtractorTests
    {
        // Dependencies
        private IRelativePathExtractor _relativePathExtractor;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _relativePathExtractor = new RelativePathExtractor();
        }

        // string GetRelativePathWithoutFilename(string rootDirectory, string fullPath);
        [Test]
        public void GetRelativePathWithoutFilename_Should_ThrowExceptionIfRootDirectoryIsNull()
        {
            string nullRootDirectory = null;
            var fullPath = string.Empty;

            var getRelativePathAction = new Action(() => _relativePathExtractor.GetRelativePathWithoutFilename(nullRootDirectory, fullPath));

            getRelativePathAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetRelativePathWithoutFilename_Should_ThrowExceptionIfFullPathIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            string fullPath = isNull
                ? null
                : string.Empty;

            var getRelativePathAction = new Action(() => _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, fullPath));

            getRelativePathAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GetRelativePathWithoutFilename_Should_ReturnFullPathIfRootDirectoryIsEmpty()
        {
            var rootDirectory = string.Empty;
            var fullPath = Directory.GetCurrentDirectory();

            var relativePath = _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, fullPath);

            relativePath.Should().Be(fullPath);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetRelativePathWithoutFilename_Should_ReturnRelativePathWithoutFilename(bool rootDirectoryEndsWithPathSeparatorChar)
        {
            var rootDirectory = rootDirectoryEndsWithPathSeparatorChar
                ? $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}"
                : Directory.GetCurrentDirectory();
            var subdirs = new[] { rootDirectory, "Path", "To", "Subdirectory", "MyFile.txt" };
            var fullPath = Path.Combine(subdirs);

            var relativePath = _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, fullPath);

            // example: "Path\To\Subdirectory\"
            var expectedPath = $"{Path.Combine("Path", "To", "Subdirectory")}{Path.DirectorySeparatorChar}";
            relativePath.Should().Be(expectedPath);
        }

    }
}
