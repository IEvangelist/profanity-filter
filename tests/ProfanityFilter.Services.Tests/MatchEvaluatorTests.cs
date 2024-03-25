// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ProfanityFilter.Services.Filters;

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class MatchEvaluatorTests
{
    [TestMethod]
    [DataRow(FilterTarget.Body, "Test", @"\*\*\*\*")]
    [DataRow(FilterTarget.Body, "swear", @"\*\*\*\*\*")]
    [DataRow(FilterTarget.Title, "swear", @"*****")]
    public void AsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.AsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Asterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void RandomAsteriskEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.RandomAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Asterisk, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreNotEqual(input, actual);
        Assert.IsTrue(actual.Length > 0 && actual.Length <= input.Length * 2 /* escape characters */);
    }

    [TestMethod]
    public void GrawlixEvaluator_Returns_Expected_Result()
    {
        var input = "someReallyLongSwearWordOrPhrase";
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.GrawlixEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Grawlix, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreNotEqual(input, actual);
        // CollectionAssert.That.Single(actual.Where(@char => @char is '$'));
    }

    [TestMethod]
    public void BoldGrawlixEvaluator_Returns_Expected_Result()
    {
        var input = "swear";
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.BoldGrawlixEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.BoldGrawlix, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreNotEqual(input, actual);
        CollectionAssert.That.Single(actual.Where(@char => @char is '$'));
    }

    [TestMethod]
    public void EmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.EmojiEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Emoji, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreNotEqual(input, actual);
    }

    [TestMethod]
    [DataRow(FilterTarget.Body, "Test", @"T\*st")]
    [DataRow(FilterTarget.Body, "swear", @"sw\*\*r")]
    [DataRow(FilterTarget.Title, "Test", @"T*st")]
    [DataRow(FilterTarget.Title, "swear", @"sw**r")]
    public void VowelAsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.VowelAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.VowelAsterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(FilterTarget.Body, "Test", @"T\*\*t")]
    [DataRow(FilterTarget.Body, "swear", @"s\*\*\*r")]
    [DataRow(FilterTarget.Title, "Test", @"T**t")]
    [DataRow(FilterTarget.Title, "swear", @"s***r")]
    public void MiddleAsteriskEvaluator_Returns_Expected_Result(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleAsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleAsterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("Test", @"bleep")]
    [DataRow("swear", @"bleep")]
    public void BleepEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.BleepEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("Test", @"████")]
    [DataRow("swear", @"█████")]
    public void RedactedBlackRectangleEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.RedactedRectangleEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.RedactedRectangle, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("Test", @"~~Test~~")]
    [DataRow("swear", @"~~swear~~")]
    public void StrikeThroughEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.StrikeThroughEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.StrikeThrough, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("Test", @"____")]
    [DataRow("swear", @"_____")]
    public void UnderscoresEvaluator_Returns_Expected_Result(string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.UnderscoresEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Underscores, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void MiddleSwearEmojiEvaluator_Returns_Expected_Result()
    {
        var input = "Test";
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.MiddleSwearEmojiEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.MiddleSwearEmoji, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreNotEqual(input, actual);
    }
}

internal partial class TestRegex
{
    [GeneratedRegex("(.+)")]
    internal static partial Regex Instance();
}
