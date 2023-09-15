// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void ToGitHubId_ReturnsCorrectId()
    {
        // Arrange
        var input = "octocat";
        var expectedOutput = new ID("octocat");

        // Act
        var output = input.ToGitHubId();

        // Assert
        Assert.Equal(expectedOutput, output);
    }
}