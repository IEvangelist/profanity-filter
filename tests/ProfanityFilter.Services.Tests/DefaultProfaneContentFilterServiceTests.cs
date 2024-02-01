// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class DefaultProfaneContentFilterServiceTests
{
    private readonly IProfaneContentFilterService _sut;

    public DefaultProfaneContentFilterServiceTests() => _sut = new DefaultProfaneContentFilterService(
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
    public async Task FilterProfanityAsync_Returns_Expected_Result(string? input, string? expectedResult)
    {
        // Act
        var result = await _sut.FilterProfanityAsync(input!,
            new(ReplacementStrategy.Asterisk, FilterTarget.Body));

        // Assert
        Assert.Equal(expectedResult, result.FinalOutput ?? input);
    }

    [Fact]
    public async Task FilterProfanityAsyncWithEmoji_Returns_Valid_Result()
    {
        var input = "This is fucking bullshit!";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.Emoji, FilterTarget.Body));

        // Assert
        Assert.NotEqual(input, result.FinalOutput);
    }

    [Fact]
    public async Task FilterProfanityAsyncWithMiddleAsterisk_ReturnsMultiStep_Result()
    {
        var input = "Lots of fucking words like manky and arrusa!";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body));

        // Assert
        Assert.True(result.IsFiltered);

        Assert.Equal(@"Lots of f\*\*\*\*\*g words like m\*\*\*y and a\*\*\*\*a!", result.FinalOutput);

        Assert.Equal(9, result.Steps.Count);
        Assert.Equal(3, result.Steps.Count(static step => step.IsFiltered));

        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("GoogleBannedWords.txt"));
        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("BritishSwearWords.txt"));
        Assert.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("ItalianSwearWords.txt"));
    }
}