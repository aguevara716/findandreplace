using System;
using System.Collections.Generic;
using System.Linq;
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

        // string NormalizeNewlines(this string @string, string desiredLineEnding)
        [Test]
        public void NormalizeNewlines_Should_ThrowExceptionForNullInput()
        {
            string inputString = null;

            var normalizeNewlinesAction = new Action(() => inputString.NormalizeNewlines());

            normalizeNewlinesAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NormalizeNewlines_Should_ReturnEmptyStringForEmptyInput()
        {
            var inputString = string.Empty;

            var normalizedString = inputString.NormalizeNewlines();

            normalizedString.Should().BeEmpty();
        }

        [Test]
        public void NormalizeNewlines_Should_ReplaceVariousNewlineCharactersWithDesiredNewlineCharacter()
        {
            var desiredNewlineCharacter = Environment.NewLine;
            var blacklistedNewlineCharacters = new List<string> { "\r\n", "\n\r", "\n", "\r" };
            var inputString = $"asdf{string.Join("asdf", blacklistedNewlineCharacters)}asdf";
            blacklistedNewlineCharacters.Remove(desiredNewlineCharacter);

            var normalizedString = inputString.NormalizeNewlines();

            normalizedString.Should().Contain(desiredNewlineCharacter);
            foreach(var blacklistedNewlineCharacter in blacklistedNewlineCharacters)
            {
                normalizedString.Should().NotMatchRegex($@"\w{blacklistedNewlineCharacter}\w");
            }
        }

        // string[] SplitAndTrim(this string @string, string delimiter)
        [Test]
        public void SplitAndTrim_Should_ThrowExceptionIfInputStringIsNull()
        {
            string inputString = null;
            var separator = "|";

            var splitAndTrimAction = new Action(() => inputString.SplitAndTrim(separator));

            splitAndTrimAction.Should().Throw<Exception>();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SplitAndTrim_Should_ReturnOriginalStringIfSeparatorIsNullOrEmpty(bool isNull)
        {
            var inputString = "abc|def";
            var separator = isNull
                ? null
                : string.Empty;

            var splitString = inputString.SplitAndTrim(separator);

            splitString.Should().HaveCount(1);
            splitString[0].Should().Be(inputString);
        }

        [Test]
        public void SplitAndTrim_Should_SplitInputStringViaSeparator()
        {
            var firstString = "asdf";
            var secondString = "jkl;";
            var separator = ",";
            var inputString = $"{firstString}{separator}{secondString}";

            var splitString = inputString.SplitAndTrim(separator);

            splitString.Should().HaveCount(2);
            splitString[0].Should().Be(firstString);
            splitString[1].Should().Be(secondString);
        }

        [Test]
        public void SplitAndTrim_Should_TrimEachSubstring()
        {
            var firstString = "asdf";
            var secondString = "jkl;";
            var separator = ",";
            var inputString = $"   {firstString}    {separator} {secondString}   ";

            var splitString = inputString.SplitAndTrim(separator);

            splitString.Should().HaveCount(2);
            splitString[0].Should().Be(firstString);
            splitString[1].Should().Be(secondString);
        }

        // string[] SplitOnNewline(this string @string)
        [Test]
        public void SplitOnNewline_Should_ThrowExceptionForNullStrings()
        {
            string @string = null;

            var splitOnNewlineAction = new Action(() => @string.SplitOnNewline());

            splitOnNewlineAction.Should().Throw<Exception>();
        }

        [Test]
        public void SplitOnNewline_Should_ReturnEmptyStringForEmptyStrings()
        {
            var @string = string.Empty;

            var lines = @string.SplitOnNewline();

            lines.Should().HaveCount(1);
            lines.First().Should().Be(string.Empty);
        }

        [Test]
        public void SplitOnNewline_Should_ReturnInputStringIfStringHasNoNewlineCharacters()
        {
            var @string = "asdf";

            var lines = @string.SplitOnNewline();

            lines.Should().HaveCount(1);
            lines.First().Should().Be(@string);
        }

        [Test]
        public void SplitOnNewline_Should_SplitByEnvironmentNewline()
        {
            var firstLine = @"`1234567890-=";
            var secondLine = @"qwertyuiop[]\";
            var thirdLine = @"asdfghjkl;'";
            var fourthLine = @"zxcvbnm,./";
            var fifthLine = "1234567890";
            var @string = $"{firstLine}{Environment.NewLine}{secondLine}{Environment.NewLine}{thirdLine}{Environment.NewLine}{fourthLine}{Environment.NewLine}{fifthLine}";

            var lines = @string.SplitOnNewline();

            lines.Should().HaveCount(5);
            lines[0].Should().Be(firstLine);
            lines[1].Should().Be(secondLine);
            lines[2].Should().Be(thirdLine);
            lines[3].Should().Be(fourthLine);
            lines[4].Should().Be(fifthLine);
        }

        // string[] TrimItemsInArray(this string[] stringArray)
        [Test]
        public void TrimItemsInArray_Should_ThrowExceptionIfInputArrayIsNull()
        {
            string[] inputArray = null;

            var trimItemsInArrayAction = new Action(() => inputArray.TrimItemsInArray());

            trimItemsInArrayAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TrimItemsInArray_Should_HandleEmptyInputArray()
        {
            var inputArray = Enumerable.Empty<string>().ToArray();

            var trimmedArray = inputArray.TrimItemsInArray();

            trimmedArray.Should().BeEmpty();
        }

        [Test]
        public void TrimItemsInArray_Should_ReturnOriginalArrayIfTrimNotNecessary()
        {
            var inputArray = new[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var trimmedArray = inputArray.TrimItemsInArray();

            trimmedArray.Should().BeEquivalentTo(inputArray);
        }

        [Test]
        public void TrimItemsInArray_Should_TrimItemsInInputArray()
        {
            var firstItem = "asdf";
            var secondItem = "jkl;";
            var inputArray = new[]
            {
                $" {firstItem}      ",
                $"   {secondItem} "
            };

            var trimmedArray = inputArray.TrimItemsInArray();

            trimmedArray.Should().HaveSameCount(inputArray);
            trimmedArray[0].Should().Be(firstItem);
            trimmedArray[1].Should().Be(secondItem);
        }

    }
}
