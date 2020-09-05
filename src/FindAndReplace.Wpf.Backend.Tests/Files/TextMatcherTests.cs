using FindAndReplace.Wpf.Backend.Files;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class TextMatcherTests
    {
        // Dependencies
        private ITextMatcher _textMatcher;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _textMatcher = new TextMatcher();
        }

        //TextMatcherResult FindTextInFile(string filePath, string findText, string content, bool isRegexSearch, bool isUsingEscapeCharacters, bool isCaseSensitive);
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FindTextInFile_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            string filePath = isNull
                ? null
                : string.Empty;
            var findText = "asdf";
            var content = "asdf";
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FindTextInFile_Should_ReturnFailureIfFindTextIsNullOrEmpty(bool isNull)
        {
            var filePath = "asdf";
            string findText = isNull
                ? null
                : string.Empty;
            var content = "asdf";
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FindTextInFile_Should_ReturnFailureIfContentIsNull()
        {
            string filePath = "asdf";
            var findText = "asdf";
            string content = null;
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeFalse();
            textMatcherResult.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void FindTextInFile_Should_ReturnSuccessIfContentIsEmpty()
        {
            var filePath = "asdf";
            var findText = "asdf";
            var content = string.Empty;
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeTrue();
            textMatcherResult.TextMatches.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public void FindTextInFile_Should_ReturnSingleMatch()
        {
            var filePath = "asdf";
            var findText = "asdf";
            var content = "asdf";
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeTrue();
            textMatcherResult.TextMatches.Should().HaveCount(1);
        }

        [Test]
        public void FindTextInFile_Should_ReturnMultipleMatches()
        {
            var filePath = "asdf";
            var findText = "a";
            var content = "aaaa";
            var isRegexSearch = false;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeTrue();
            textMatcherResult.TextMatches.Should().HaveCount(4);
        }

        [Test]
        public void FindTextInFile_Should_HandleRegularExpressions()
        {
            var filePath = "asdf";
            var findText = "[a-z][0-9]";
            var content = "abcd123abcd123";
            var isRegexSearch = true;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = false;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeTrue();
            textMatcherResult.TextMatches.Should().HaveCount(2);
        }

        [Test]
        public void FindTextInFile_Should_CaseSensitiveSearches()
        {
            var filePath = "asdf";
            var findText = "A";
            var content = "Aaaa";
            var isRegexSearch = true;
            var isUsingEscapeCharacters = false;
            var isCaseSensitive = true;

            var textMatcherResult = _textMatcher.FindTextInFile(filePath, findText, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);

            textMatcherResult.IsSuccessful.Should().BeTrue();
            textMatcherResult.TextMatches.Should().HaveCount(1);
        }

    }
}
