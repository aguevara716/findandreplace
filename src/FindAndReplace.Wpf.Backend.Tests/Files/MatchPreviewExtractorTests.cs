using System.Collections.Generic;
using System.Linq;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Results;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class MatchPreviewExtractorTests
    {
        // Dependencies
        private IMatchPreviewExtractor _matchPreviewExtractor;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _matchPreviewExtractor = new MatchPreviewExtractor();
        }

        // Private Methods

        // MatchPreviewExtractionResult ExtractMatchPreviews(string filePath, string fileContent, IList<TextMatch> textMatches);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ExtractMatchPreviews_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;
            var fileContent = "asdf";
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnFailureIfFileContentIsNull()
        {
            var filePath = "asdf";
            string fileContent = null;
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnFailureIfTextMatchesIsNull()
        {
            var filePath = "asdf";
            var fileContent = "asdf";
            List<TextMatch> textMatches = null;

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeFalse();
            matchPreviewExtractionResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnSuccessWithEmptyPayloadIfFileContentIsEmpty()
        {
            var filePath = "asdf";
            var fileContent = string.Empty;
            List<TextMatch> textMatches = new List<TextMatch>() { new TextMatch { Length = 1, StartIndex = 1 } };

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeTrue();
            matchPreviewExtractionResult.Previews.Should().BeEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_ReturnSuccessWithEmptyPayloadIfTextMatchCollectionIsEmpty()
        {
            var filePath = "asdf";
            var fileContent = "asdf";
            var textMatches = new List<TextMatch>();

            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            matchPreviewExtractionResult.IsSuccessful.Should().BeTrue();
            matchPreviewExtractionResult.Previews.Should().BeEmpty();
        }

        [Test]
        public void ExtractMatchPreviews_Should_Thing()
        {
            // Data is from a real execution of fnr.exe
            var filePath = "asdf";
            var fileContent = "using EnergyPi.Web.Builders;\r\nusing EnergyPi.Web.Data;\r\nusing EnergyPi.Web.DataServices;\r\nusing EnergyPi.Web.Entities;\r\nusing EnergyPi.Web.Repositories;\r\nusing Microsoft.AspNetCore.Builder;\r\nusing Microsoft.AspNetCore.Hosting;\r\nusing Microsoft.AspNetCore.Identity;\r\nusing Microsoft.EntityFrameworkCore;\r\nusing Microsoft.Extensions.Configuration;\r\nusing Microsoft.Extensions.DependencyInjection;\r\nusing Microsoft.Extensions.Hosting;\r\n\r\nnamespace EnergyPi.Web\r\n{\r\n    public class Startup\r\n    {\r\n        public Startup(IConfiguration configuration)\r\n        {\r\n";
            var textMatches = new List<TextMatch>
            {
                new TextMatch
                {
                    Length = 5,
                    StartIndex = 473
                }
            };
            var expectedPreviewText = "14 namespace EnergyPi.Web\r\n15 {\r\n16     public class Startup\r\n17     {\r\n18         public Startup(IConfiguration configuration)\r\n";

            var actualPreviewResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath, fileContent, textMatches);

            actualPreviewResult.IsSuccessful.Should().BeTrue();
            actualPreviewResult.Previews.First().Content.Should().Be(expectedPreviewText);
        }

    }
}
