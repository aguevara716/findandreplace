using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.Mappers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Tests.Mappers
{
    [TestFixture]
    public class FileResultMappers
    {
        // Constants
        private const string EXTENSION = ".txt";

        // Dependencies
        private IRelativePathExtractor _relativePathExtractor;
        private IFileResultMapper _fileResultMapper;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _relativePathExtractor = Substitute.For<IRelativePathExtractor>();
            _fileResultMapper = new FileResultMapper(_relativePathExtractor);
        }

        // Private Methods
        private IEnumerable<string> GenerateFilePaths(string rootDirectory, int count)
        {
            var filePathsDictionary = new ConcurrentDictionary<string, bool>();
            Parallel.ForEach(Enumerable.Range(0, count), index =>
            {
                var path = Path.Combine(rootDirectory, "InnerDirectory", $"File-{index}{EXTENSION}");
                filePathsDictionary.AddOrUpdate(path, true, (s, b) => true);
            });
            var filePathsCollection = filePathsDictionary.Keys.OrderBy(s => s);
            return filePathsCollection;
        }

        // FileResult Map(string rootDirectory, string filePath);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MapSingle_Should_ThrowExceptionIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            string rootDirectory = isNull
                ? null
                : string.Empty;
            var filePath = "asdf";

            var mapSingleAction = new Action(() => _fileResultMapper.Map(rootDirectory, filePath));

            mapSingleAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MapSingle_Should_ThrowExceptionIfFilePathIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            string filePath = isNull
                ? null
                : string.Empty;

            var mapSingleAction = new Action(() => _fileResultMapper.Map(rootDirectory, filePath));

            mapSingleAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void MapSingle_Should_CallRelativePathExtractor()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filePath = GenerateFilePaths(rootDirectory, 1).First();

            _ = _fileResultMapper.Map(rootDirectory, filePath);

            _relativePathExtractor.Received().GetRelativePathWithoutFilename(rootDirectory, filePath);
        }

        [Test]
        public void MapSingle_Should_ReturnFileResult()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filePath = GenerateFilePaths(rootDirectory, 1).First();
            var relativePath = "asdf";
            _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, filePath).Returns(relativePath);

            var fileResult = _fileResultMapper.Map(rootDirectory, filePath);

            fileResult.Should().NotBeNull();
            fileResult.Extension.Should().Be(Path.GetExtension(filePath));
            fileResult.Filename.Should().Be(Path.GetFileName(filePath));
            fileResult.FullPath.Should().Be(filePath);
            fileResult.RelativePath.Should().Be(relativePath);
        }

        // IList<FileResult> Map(string rootDirectory, IList<string> filePaths);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MapMany_Should_ThrowExceptionIfRootDirectoryIsNullOrEmpty(bool isNull)
        {
            string rootDirectory = isNull
                ? null
                : string.Empty;
            var filePaths = GenerateFilePaths("asdf", 10).ToList();

            var mapManyAction = new Action(() => _fileResultMapper.Map(rootDirectory, filePaths));

            mapManyAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void MapMany_Should_ThrowExceptionIfFilePathsIsNullOrEmpty(bool isNull)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            IList<string> filePaths = isNull
                ? null
                : Enumerable.Empty<string>().ToList();

            var mapManyAction = new Action(() => _fileResultMapper.Map(rootDirectory, filePaths));

            mapManyAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void MapMany_Should_MapAllFilePaths(int count)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var filePaths = GenerateFilePaths(rootDirectory, count).ToList();
            var relativePath = "Subdirectory";
            _relativePathExtractor.GetRelativePathWithoutFilename(null, null).ReturnsForAnyArgs(relativePath);

            var fileResult = _fileResultMapper.Map(rootDirectory, filePaths);

            fileResult.Should().HaveSameCount(filePaths);
            fileResult.Select(fr => fr.Extension).Distinct().Should().Contain(EXTENSION);
            fileResult.Select(fr => fr.Filename).Should().BeEquivalentTo(filePaths.Select(s => Path.GetFileName(s)));
            fileResult.Select(fr => fr.FullPath).ToList().Should().BeEquivalentTo(filePaths);
            fileResult.Select(fr => fr.RelativePath).Distinct().All(s => s == relativePath).Should().BeTrue();
        }

    }
}
