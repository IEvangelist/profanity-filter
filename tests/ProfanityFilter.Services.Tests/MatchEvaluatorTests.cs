// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public partial class MatchEvaluatorTests
{
    [Theory]
    [InlineData("Test", @"\*\*\*\*")]
    [InlineData("swear", @"\*\*\*\*\*")]
    public void AsteriskEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.AsteriskEvaluator;

        var actual = sut(match);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RandomAsteriskEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.RandomAsteriskEvaluator;

        var actual = sut(match);

        Assert.NotEqual(input, actual);
        Assert.True(actual.Length > 0 && actual.Length <= input.Length * 2 /* escape characters */);
    }

    [Fact]
    public void EmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.EmojiEvaluator;

        var actual = sut(match);

        Assert.NotEqual(input, actual);
    }

    [Theory]
    [InlineData("Test", @"T\*st")]
    [InlineData("swear", @"sw\*\*r")]
    public void VowelAsteriskEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.VowelAsteriskEvaluator;

        var actual = sut(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", @"T\*\*t")]
    [InlineData("swear", @"s\*\*\*r")]
    public void MiddleAsteriskEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleAsteriskEvaluator;

        var actual = sut(match);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MiddleSwearEmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleSwearEmojiEvaluator;

        var actual = sut(match);

        Assert.NotEqual(input, actual);
    }

    [GeneratedRegex("(.+)")]
    private static partial Regex TestRegex();
}
