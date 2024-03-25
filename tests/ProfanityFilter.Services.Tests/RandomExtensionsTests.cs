// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class RandomExtensionsTests
{
    [TestMethod]
    public void RandomItemsWithLimitsReturnsRandomSubsetOfItems()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(3, "");

        // Assert
        Assert.AreEqual(3, result.Length);
    }

    [TestMethod]
    public void RandomItemsWithLimitsRespectsLimits()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(3, "a");

        // Assert
        Assert.AreEqual(3, result.Length);
        // CollectionAssert.That.Single(result.Where(str => str is "a"));
    }

    [TestMethod]
    public void RandomItemsWithLimitsReturnsHugeArrayWithOnlyOneLimitedValue()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(100_000, "c");

        // Assert
        CollectionAssert.That.Contains(result, str => str is "a");
        CollectionAssert.That.Contains(result, str => str is "b");
        CollectionAssert.That.Single(result.Where(str => str is "c"));
        CollectionAssert.That.Contains(result, str => str is "d");
        CollectionAssert.That.Contains(result, str => str is "e");
    }
}
