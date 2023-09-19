// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class DefaultProfaneContentCensorServiceTests
{
    private readonly IProfaneContentCensorService _sut;

    public DefaultProfaneContentCensorServiceTests() => _sut = new DefaultProfaneContentCensorService();

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("This is a sentence with the word crap.", true)]
    [InlineData("This is a sentence with the word CrAp.", true)]
    [InlineData("This is a sentence with the word crap and shit.", true)]
    [InlineData("This is a sentence with the word crap and shit and fuck.", true)]
    [InlineData("This is a sentence with the word crap and shit and fuck and ass.", true)]
    public async Task ContainsProfanityAsync_Returns_Expected_Result(string input, bool expectedResult)
    {
        // Act
        var result = await _sut.ContainsProfanityAsync(input);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("This is a clean sentence.", "This is a clean sentence.")]
    [InlineData("This is a sentence with the word crap.", "This is a sentence with the word ****.")]
    [InlineData("This is a sentence with the word CrAp.", "This is a sentence with the word ****.")]
    [InlineData("This is a sentence with the word crap and shit.", "This is a sentence with the word **** and ****.")]
    [InlineData("This is a sentence with the word crap and shit and fuck.", "This is a sentence with the word **** and **** and ****.")]
    [InlineData("This is a sentence with the word crap and shit and fuck and ass.", "This is a sentence with the word **** and **** and **** and ***.")]
    public async Task CensorProfanityAsync_Returns_Expected_Result(string input, string expectedResult)
    {
        // Act
        var result = await _sut.CensorProfanityAsync(input, ReplacementType.Asterisk);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task CensorProfanityAsyncWithEmoji_Returns_Valid_Result()
    {
        var input = "This is fucking bullshit!";

        // Act
        var result = await _sut.CensorProfanityAsync(input, ReplacementType.Emoji);

        // Assert
        Assert.NotEqual(input, result);
    }
}