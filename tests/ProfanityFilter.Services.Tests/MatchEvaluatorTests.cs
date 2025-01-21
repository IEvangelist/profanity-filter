// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class MatchEvaluatorTests
{
    [TestMethod]
    [DataRow(FilterTarget.Body, "Test", @"\*\*\*\*")]
    [DataRow(FilterTarget.Body, "swear", @"\*\*\*\*\*")]
    [DataRow(FilterTarget.Title, "swear", @"*****")]
    public void AsteriskEvaluatorReturnsExpectedResult(FilterTarget target, string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.AsteriskEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Asterisk, target);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void RandomAsteriskEvaluatorReturnsExpectedResult()
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
    public void GrawlixEvaluatorReturnsExpectedResult()
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
    public void BoldGrawlixEvaluatorReturnsExpectedResult()
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
    public void EmojiEvaluatorReturnsExpectedResult()
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
    public void VowelAsteriskEvaluatorReturnsExpectedResult(FilterTarget target, string input, string expected)
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
    public void MiddleAsteriskEvaluatorReturnsExpectedResult(FilterTarget target, string input, string expected)
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
    public void BleepEvaluatorReturnsExpectedResult(string input, string expected)
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
    public void RedactedBlackRectangleEvaluatorReturnsExpectedResult(string input, string expected)
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
    public void StrikeThroughEvaluatorReturnsExpectedResult(string input, string expected)
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
    public void UnderscoresEvaluatorReturnsExpectedResult(string input, string expected)
    {
        var regex = TestRegex.Instance();
        var match = regex.Match(input);
        var sut = MatchEvaluators.UnderscoresEvaluator;

        var parameters = new FilterParameters(ReplacementStrategy.Underscores, FilterTarget.Body);
        var actual = sut(parameters).Invoke(match);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void MiddleSwearEmojiEvaluatorReturnsExpectedResult()
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

internal sealed partial class TestRegex
{
    [GeneratedRegex("(.+)")]
    internal static partial Regex Instance();
}
