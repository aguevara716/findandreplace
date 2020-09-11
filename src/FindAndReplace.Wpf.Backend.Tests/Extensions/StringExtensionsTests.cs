using System;
using FindAndReplace.Wpf.Backend.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        // string ConvertWildcardPatternToRegexPattern(this string wildcardPattern)
        [Test]
        public void ConvertWildcardPatternToRegexPattern_Should_HandleEmptyStrings()
        {
            var wildcardPattern = string.Empty;
            var expectedRegexPattern = "^$";

            var actualRegexPattern = wildcardPattern.ConvertWildcardPatternToRegexPattern();

            actualRegexPattern.Should().Be(expectedRegexPattern);
        }

        [Test]
        public void ConvertWildcardPatternToRegexPattern_Should_ThrowExceptionForNullStrings()
        {
            string wildcardPattern = null;

            var convertAction = new Action(() => wildcardPattern.ConvertWildcardPatternToRegexPattern());

            convertAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ConvertWildcardPatternToRegexPattern_Should_HandleAsterisks()
        {
            var wildcardPattern = "*.cs";
            var expectedRegexPattern = @"^.*\.cs$";

            var actualRegexPattern = wildcardPattern.ConvertWildcardPatternToRegexPattern();

            actualRegexPattern.Should().Be(expectedRegexPattern);
        }

        [Test]
        public void ConvertWildcardPatternToRegexPattern_Should_HandleQuestionMarks()
        {
            var wildcardPattern = "?.cs";
            var expectedRegexPattern = @"^.\.cs$";

            var actualRegexPattern = wildcardPattern.ConvertWildcardPatternToRegexPattern();

            actualRegexPattern.Should().Be(expectedRegexPattern);
        }

        // bool IsEmpty(this string @string)
        [Test]
        public void IsEmpty_Should_ThrowExceptionIfStringIsNull()
        {
            string nullString = null;

            var isEmptyAction = new Action(() => nullString.IsEmpty());

            isEmptyAction.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void IsEmpty_Should_ReturnTrueForEmptyStrings()
        {
            var @string = string.Empty;

            var isEmpty = @string.IsEmpty();

            isEmpty.Should().BeTrue();
        }

        [Test]
        public void IsEmpty_Should_ReturnFalseForNonEmptyStrings()
        {
            var @string = "a";

            var isEmpty = @string.IsEmpty();

            isEmpty.Should().BeFalse();
        }

        [Test]
        public void IsEmpty_Should_ReturnFalseForWhitespaceCharacters()
        {
            var @string = "\n";

            var isEmpty = @string.IsEmpty();

            isEmpty.Should().BeFalse();
        }

    }
}
