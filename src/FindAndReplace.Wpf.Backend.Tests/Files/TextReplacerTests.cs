using System;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Parameters;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    [TestFixture]
    public class TextReplacerTests
    {
        // Dependencies
        private ITextReplacer _textReplacer;

        // Initialization
        [SetUp]
        public void BeforeEach()
        {
            _textReplacer = new TextReplacer();
        }

        // TextReplacementResult ReplaceText(TextReplacementParameters textReplacementParameters)
        [Test]
        public void ReplaceText_Should_ReturnFailureIfParametersAreNull()
        {
            TextReplacementParameters textReplacementParameters = null;

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.IsSuccessful.Should().BeFalse();
            textReplacementResult.UpdatedContent.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_ReturnFailureIfFileContentIsNullOrEmpty(bool isNull)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = isNull ? null : string.Empty,
                FilePath = Guid.NewGuid().ToString(),
                FindText = Guid.NewGuid().ToString(),
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = Guid.NewGuid().ToString()
            };

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.IsSuccessful.Should().BeFalse();
            textReplacementResult.UpdatedContent.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_ReturnFailureIfFilePathIsNullOrEmpty(bool isNull)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = Guid.NewGuid().ToString(),
                FilePath = isNull ? null : string.Empty,
                FindText = Guid.NewGuid().ToString(),
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = Guid.NewGuid().ToString()
            };

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.IsSuccessful.Should().BeFalse();
            textReplacementResult.UpdatedContent.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_ReturnFailureIfFindTextIsNullOrEmpty(bool isNull)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = Guid.NewGuid().ToString(),
                FilePath = Guid.NewGuid().ToString(),
                FindText = isNull ? null : string.Empty,
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = Guid.NewGuid().ToString()
            };

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.IsSuccessful.Should().BeFalse();
            textReplacementResult.UpdatedContent.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_ReturnFailureIfReplaceTextIsNullOrEmpty(bool isNull)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = Guid.NewGuid().ToString(),
                FilePath = Guid.NewGuid().ToString(),
                FindText = Guid.NewGuid().ToString(),
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = isNull ? null : string.Empty
            };

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().NotBeNullOrEmpty();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.IsSuccessful.Should().BeFalse();
            textReplacementResult.UpdatedContent.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_HandleSimpleStringReplacement(bool isCaseSensitive)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = isCaseSensitive 
                    ? "i have a BLUE blue car" 
                    : "i have a blue blue car",
                FilePath = Guid.NewGuid().ToString(),
                FindText = isCaseSensitive 
                    ? "BLUE" 
                    : "blue",
                IsCaseSensitive = isCaseSensitive,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = "white"
            };
            var expectedContent = isCaseSensitive 
                ? "i have a white blue car" 
                : "i have a white white car";

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().BeNullOrEmpty();
            textReplacementResult.IsSuccessful.Should().BeTrue();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.UpdatedContent.Should().Be(expectedContent);
        }

        [Test]
        public void ReplaceText_Should_HandleSimpleStringReplacementWithEscapeCharacters()
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = "there should be a newline after the previous word \"newline\"",
                FilePath = Guid.NewGuid().ToString(),
                FindText = " newline ",
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = true,
                ReplaceText = " newline\\n"
            };
            var expectedContent = "there should be a newline\nafter the previous word \"newline\"";

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().BeNullOrEmpty();
            textReplacementResult.IsSuccessful.Should().BeTrue();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.UpdatedContent.Should().Be(expectedContent);
        }

        [Test]
        public void ReplaceText_Should_HandleSimpleStringReplacementAndIgnoreEscapeCharacters()
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = "there should be a newline after the previous word \"newline\"",
                FilePath = Guid.NewGuid().ToString(),
                FindText = " newline ",
                IsCaseSensitive = false,
                IsRegexSearch = false,
                IsUsingEscapeCharacters = false,
                ReplaceText = " newline\\n"
            };
            var expectedContent = "there should be a newline\\nafter the previous word \"newline\"";

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().BeNullOrEmpty();
            textReplacementResult.IsSuccessful.Should().BeTrue();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.UpdatedContent.Should().Be(expectedContent);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReplaceText_Should_HandleRegularExpressions(bool isCaseSensitive)
        {
            var textReplacementParameters = new TextReplacementParameters
            {
                FileContent = "aaaa AAAA bbbb BBBB cccc CCCC dddd DDDD",
                FilePath = Guid.NewGuid().ToString(),
                FindText = "A",
                IsCaseSensitive = isCaseSensitive,
                IsRegexSearch = true,
                IsUsingEscapeCharacters = false,
                ReplaceText = "Z"
            };
            var expectedContent = isCaseSensitive 
                ? "aaaa ZZZZ bbbb BBBB cccc CCCC dddd DDDD" 
                : "ZZZZ ZZZZ bbbb BBBB cccc CCCC dddd DDDD";

            var textReplacementResult = _textReplacer.ReplaceText(textReplacementParameters);

            textReplacementResult.ErrorMessage.Should().BeNullOrEmpty();
            textReplacementResult.IsSuccessful.Should().BeTrue();
            textReplacementResult.Exception.Should().BeNull();
            textReplacementResult.UpdatedContent.Should().Be(expectedContent);
        }

    }
}
