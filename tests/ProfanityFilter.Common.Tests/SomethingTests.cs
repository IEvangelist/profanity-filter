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
    public void SomethingShouldBeAssignableToIMaybe()
    {
        // Arrange
        var something = new Available<int>(42);

        // Act & Assert
        Assert.IsInstanceOfType<IMaybe<int>>(something);
        Assert.AreEqual(42, something.Value);
    }
}
