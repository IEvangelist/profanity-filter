// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class RandomExtensionsTests
{
    [Fact]
    public void RandomItemsWithLimitsReturnsRandomSubsetOfItems()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(3, "");

        // Assert
        Assert.Equal(3, result.Length);
    }

    [Fact]
    public void RandomItemsWithLimitsRespectsLimits()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(3, "a");

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Single(result.Where(str => str is "a"));
    }

    [Fact]
    public void RandomItemsWithLimitsReturnsHugeArrayWithOnlyOneLimitedValue()
    {
        // Arrange
        var source = new[] { "a", "b", "c", "d", "e" };

        // Act
        var result = source.RandomItemsWithLimitToOne(100_000, "c");

        // Assert
        Assert.Contains(result, str => str is "a");
        Assert.Contains(result, str => str is "b");
        Assert.Single(result.Where(str => str is "c"));
        Assert.Contains(result, str => str is "d");
        Assert.Contains(result, str => str is "e");
    }
}
