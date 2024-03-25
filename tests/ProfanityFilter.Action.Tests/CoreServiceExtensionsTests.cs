// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

[TestClass]
public class CoreServiceExtensionsTests
{
    [TestMethod]
    [DataRow(null, null)]
    [DataRow("WebForms,WinForms", new string[] { "WebForms", "WinForms" })]
    public void CoreServiceCorrectlyParseManualWordList(string? input, string[]? expected = null)
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.ManualProfaneWords] = input ?? ""
        });

        var actual = sut.GetManualProfaneWords();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task CoreServiceCorrectlyParseCustomWordList()
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.CustomProfaneWordsUrl] =
                "https://gist.githubusercontent.com/IEvangelist/355ad7852bafedb4365a896d1c545a6c/raw/cbd4cea1592ab5acb518d139240dd5a11f42612c/Example.ProfaneWord.List.txt"
        });

        var actual = await sut.GetCustomProfaneWordsAsync();

        CollectionAssert.AreEqual(
        new string[] {
            "WebForms",
            "Web Forms",
            "ASP.NET WebForms",
            "ASP.NET Web Forms",
            "ASP.NET Core WebForms",
            "ASP.NET Core Web Forms",
            "WinForms",
            "WinForms",
            "WindowsForms",
            "Windows Forms",
        }, actual);
    }

    [TestMethod]
    [DataRow("emoji", ReplacementStrategy.Emoji)]
    [DataRow("EMOJI", ReplacementStrategy.Emoji)]
    [DataRow("eMoJi", ReplacementStrategy.Emoji)]
    [DataRow("anger-emoji", ReplacementStrategy.AngerEmoji)]
    [DataRow("angerEmoji", ReplacementStrategy.AngerEmoji)]
    [DataRow("AngerEmoji", ReplacementStrategy.AngerEmoji)]
    [DataRow("first-letter-then-asterisk", ReplacementStrategy.FirstLetterThenAsterisk)]
    [DataRow(nameof(ReplacementStrategy.FirstLetterThenAsterisk), ReplacementStrategy.FirstLetterThenAsterisk)]
    public void CoreServiceCorrectlyParsesReplacementStrategy(string input, ReplacementStrategy expected)
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.ReplacementStrategy] = input
        });

        var actual = sut.GetReplacementStrategy();

        Assert.AreEqual(expected, actual);
    }
}
