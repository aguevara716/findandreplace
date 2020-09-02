using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;

namespace FindAndReplace.Wpf.Backend.Tests.Extensions
{
    [TestFixture]
    public class IEnumerableGenericExtensionsTests
    {
        [Test]
        public void IsNullOrEmpty_Should_ReturnTrueForNullCollections()
        {
            IEnumerable<int> collection = null;

            var isNullOrEmpty = collection.IsNullOrEmpty();

            isNullOrEmpty.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmpty_Should_ReturnTrueForEmptyCollections()
        {
            var collection = Enumerable.Empty<int>();

            var isNullOrEmpty = collection.IsNullOrEmpty();

            isNullOrEmpty.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmpty_Should_ReturnFalseForNonEmptyCollections()
        {
            var collection = Enumerable.Range(1, 100);

            var isNullOrEmpty = collection.IsNullOrEmpty();

            isNullOrEmpty.Should().BeFalse();
        }

    }
}
