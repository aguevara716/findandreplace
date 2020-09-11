using System;
using FindAndReplace.Wpf.Backend.Results;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Results
{
    [TestFixture]
    public class BaseResultTests
    {
        // string GetErrorText()
        [Test]
        public void GetErrorText_Should_ReturnEmptyStringIfResultIsSuccess()
        {
            var testResult = TestResult.CreateSuccess<TestResult>("file", 123);

            var errorText = testResult.GetErrorText();

            errorText.Should().BeEmpty();
        }

        [Test]
        public void GetErrorText_Should_ReturnEmptyStringIfErrorMessageAndExceptionAreEmpty()
        {
            var testResult = TestResult.CreateFailure<TestResult>("file", string.Empty, null);

            var errorText = testResult.GetErrorText();

            errorText.Should().BeEmpty();
        }

        [Test]
        public void GetErrorText_Should_OnlyContainErrorMessageIfExceptionIsNotProvided()
        {
            var errorMessage = Guid.NewGuid().ToString();
            var testResult = TestResult.CreateFailure<TestResult>("file", errorMessage);

            var errorText = testResult.GetErrorText();

            errorText.Should().Be(errorMessage);
        }

        [Test]
        public void GetErrorText_Should_OnlyContainErrorMessageIfExceptionIsNull()
        {
            var errorMessage = Guid.NewGuid().ToString();
            var testResult = TestResult.CreateFailure<TestResult>("file", errorMessage, null);

            var errorText = testResult.GetErrorText();

            errorText.Should().Be(errorMessage);
        }

        [Test]
        public void GetErrorText_Should_OnlyContainExceptionMessageIfErrorMessageIsNotSpecified()
        {
            var exception = new Exception(Guid.NewGuid().ToString());
            var testResult = TestResult.CreateFailure<TestResult>("file", exception);

            var errorText = testResult.GetErrorText();

            errorText.Should().Be(exception.Message);
        }

        [Test]
        public void GetErrorText_Should_OnlyContainExceptionMessageIfErrorMessageIsNullOrEmpty(bool isNull)
        {
            var exception = new Exception(Guid.NewGuid().ToString());
            string errorMessage = isNull
                ? null
                : string.Empty;
            var testResult = TestResult.CreateFailure<TestResult>("file", errorMessage, exception);

            var errorText = testResult.GetErrorText();

            errorText.Should().Be(exception.Message);
        }

        [Test]
        public void GetErrorText_Should_ContainErrorMessageAndExceptionMethodIfBothAreProvided()
        {
            var errorMessage = Guid.NewGuid().ToString();
            var exception = new Exception(Guid.NewGuid().ToString());
            var testResult = TestResult.CreateFailure<TestResult>("file", errorMessage, exception);

            var errorText = testResult.GetErrorText();

            errorText.Should().Be($"{errorMessage}{Environment.NewLine}{exception.Message}");
        }
    }

    internal class TestResult : BaseResult<int>
    {

    }
}
