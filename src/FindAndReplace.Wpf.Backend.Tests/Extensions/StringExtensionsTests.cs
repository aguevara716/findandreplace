using System;
using FindAndReplace.Wpf.Backend.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
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

    }
}
