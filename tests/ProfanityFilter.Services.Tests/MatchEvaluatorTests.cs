// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ProfanityFilter.Services.Filters;

namespace ProfanityFilter.Services.Tests;

public partial class MatchEvaluatorTests
{
    [Theory]
    [InlineData(FilterTarget.Body, "Test", @"\*\*\*\*")]
    [InlineData(FilterTarget.Body, "swear", @"\*\*\*\*\*")]
    [InlineData(FilterTarget.Title, "swear", @"*****")]
    public void AsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.AsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Asterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RandomAsteriskEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.RandomAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Asterisk, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.NotEqual(input, actual);
        Assert.True(actual.Length > 0 && actual.Length <= input.Length * 2 /* escape characters */);
    }

    [Fact]
    public void GrawlixEvaluator_Returns_Expected_Result()
    {
        var input = "swear";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.GrawlixEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Grawlix, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.NotEqual(input, actual);
    }

    [Fact]
    public void BoldGrawlixEvaluator_Returns_Expected_Result()
    {
        var input = "swear";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.BoldGrawlixEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.BoldGrawlix, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.NotEqual(input, actual);
    }

    [Fact]
    public void EmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.EmojiEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Emoji, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.NotEqual(input, actual);
    }

    [Theory]
    [InlineData(FilterTarget.Body, "Test", @"T\*st")]
    [InlineData(FilterTarget.Body, "swear", @"sw\*\*r")]
    [InlineData(FilterTarget.Title, "Test", @"T*st")]
    [InlineData(FilterTarget.Title, "swear", @"sw**r")]
    public void VowelAsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.VowelAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.VowelAsterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(FilterTarget.Body, "Test", @"T\*\*t")]
    [InlineData(FilterTarget.Body, "swear", @"s\*\*\*r")]
    [InlineData(FilterTarget.Title, "Test", @"T**t")]
    [InlineData(FilterTarget.Title, "swear", @"s***r")]
    public void MiddleAsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleAsterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", @"bleep")]
    [InlineData("swear", @"bleep")]
    public void BleepEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.BleepEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", @"████")]
    [InlineData("swear", @"█████")]
    public void RedactedBlackRectangleEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.RedactedRectangleEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.RedactedRectangle, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", @"~~Test~~")]
    [InlineData("swear", @"~~swear~~")]
    public void StrikeThroughEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.StrikeThroughEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.StrikeThrough, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", @"____")]
    [InlineData("swear", @"_____")]
    public void UnderscoresEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.UnderscoresEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Underscores, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MiddleSwearEmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleSwearEmojiEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleSwearEmoji, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.NotEqual(input, actual);
    }

    [GeneratedRegex("(.+)")]
    private static partial Regex TestRegex();
}
