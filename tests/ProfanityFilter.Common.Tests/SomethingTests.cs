// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Tests;

[TestClass]
public sealed class SomethingTests
{
    [TestMethod]
    public void SomethingShouldContainValue()
    {
        // Arrange
        var expectedValue = "test value";
        var something = new Available<string>(expectedValue);

        // Act
        var actualValue = something.Value;

        // Assert
        Assert.AreEqual(expectedValue, actualValue);
    }

    [TestMethod]
    public void SomethingShouldBeConvertibleToMaybe()
    {
        // Arrange
        Maybe<int> something = new Available<int>(42);

        // Act & Assert
        Assert.IsTrue(something is Available<int> { Value: 42 });
    }
}
