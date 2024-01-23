// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

[Trait("Category", "Integration")]
public class CustomGitHubClientTests
{
    private readonly string Token = Environment.GetEnvironmentVariable("GITHUB_TOKEN")!;

    private const string Owner = "IEvangelist";
    private const string Repo = "profanity-filter";

    [Fact(Skip = "This is primarily for local testing...")]
    public async Task GetLabelAsync_Returns_Correct_Values()
    {
        // Arrange
        var client = new CustomGitHubClient(Owner, Repo, Token);

        var label = await client.GetLabelAsync();
        Assert.NotNull(label);
        Assert.Equal(DefaultLabel.Color, label.Color);
        Assert.Equal(DefaultLabel.Description, label.Description);
        Assert.Equal(DefaultLabel.Name, label.Name);
    }
}
