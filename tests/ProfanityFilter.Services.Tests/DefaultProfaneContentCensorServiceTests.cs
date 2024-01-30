// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class DefaultProfaneContentCensorServiceTests
{
    private readonly IProfaneContentCensorService _sut;

    public DefaultProfaneContentCensorServiceTests() => _sut = new DefaultProfaneContentCensorService(
        new MemoryCache(Options.Create<MemoryCacheOptions>(new())));

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("This is a clean sentence.", "This is a clean sentence.")]
    [InlineData("This is a sentence with the word crap.", @"This is a sentence with the word \*\*\*\*.")]
    [InlineData("This is a sentence with the word CrAp.", @"This is a sentence with the word \*\*\*\*.")]
    [InlineData("This is a sentence with the word crap and shit.", @"This is a sentence with the word \*\*\*\* and \*\*\*\*.")]
    [InlineData("This is a sentence with the word crap and shit and fuck.", @"This is a sentence with the word \*\*\*\* and \*\*\*\* and \*\*\*\*.")]
    [InlineData("This is a sentence with the word crap and shit and fuck and ass.", @"This is a sentence with the word \*\*\*\* and \*\*\*\* and \*\*\*\* and \*\*\*.")]
    public async Task CensorProfanityAsync_Returns_Expected_Result(string? input, string? expectedResult)
    {
        // Act
        var result = await _sut.CensorProfanityAsync(input!, ReplacementStrategy.Asterisk);

        // Assert
        Assert.Equal(expectedResult, result.FinalOutput ?? input);
    }

    [Fact]
    public async Task CensorProfanityAsyncWithEmoji_Returns_Valid_Result()
    {
        var input = "This is fucking bullshit!";

        // Act
        var result = await _sut.CensorProfanityAsync(input, ReplacementStrategy.Emoji);

        // Assert
        Assert.NotEqual(input, result.FinalOutput);
    }

    [Fact]
    public async Task CensorProfanityAsyncWithMiddleAsterisk_ReturnsMultiStep_Result()
    {
        var input = "Lots of fucking words like manky and arrusa!";

        // Act
        var result = await _sut.CensorProfanityAsync(input, ReplacementStrategy.MiddleAsterisk);

        // Assert
        Assert.True(result.IsCensored);

        Assert.Equal(@"Lots of f\*\*\*\*\*g words like m\*\*\*y and a\*\*\*\*a!", result.FinalOutput);

        Assert.Equal(9, result.Steps.Count);
        Assert.Equal(3, result.Steps.Count(static step => step.IsCensored));

        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("GoogleBannedWords.txt"));
        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("BritishSwearWords.txt"));
        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("ItalianSwearWords.txt"));
    }
}