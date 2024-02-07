// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

public class CoreServiceExtensionsTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("WebForms,WinForms", new string[] { "WebForms", "WinForms" })]
    public void CoreServiceCorrectlyParseManualWordList(string? input, string[]? expected = null)
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.ManualProfaneWords] = input ?? ""
        });

        var actual = sut.GetManualProfaneWords();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task CoreServiceCorrectlyParseCustomWordList()
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.CustomProfaneWordsUrl] =
                "https://gist.githubusercontent.com/IEvangelist/355ad7852bafedb4365a896d1c545a6c/raw/cbd4cea1592ab5acb518d139240dd5a11f42612c/Example.ProfaneWord.List.txt"
        });

        var actual = await sut.GetCustomProfaneWordsAsync();

        Assert.Equal<string[]>(
        [
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
        ], actual);
    }

    [Theory]
    [InlineData("emoji", ReplacementStrategy.Emoji)]
    [InlineData("EMOJI", ReplacementStrategy.Emoji)]
    [InlineData("eMoJi", ReplacementStrategy.Emoji)]
    [InlineData("anger-emoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("angerEmoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("AngerEmoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("first-letter-then-asterisk", ReplacementStrategy.FirstLetterThenAsterisk)]
    [InlineData(nameof(ReplacementStrategy.FirstLetterThenAsterisk), ReplacementStrategy.FirstLetterThenAsterisk)]
    public void CoreServiceCorrectlyParsesReplacementStrategy(string input, ReplacementStrategy expected)
    {
        ICoreService sut = new TestCoreService(new()
        {
            [ActionInputs.ReplacementStrategy] = input
        });

        var actual = sut.GetReplacementStrategy();

        Assert.Equal(expected, actual);
    }
}
