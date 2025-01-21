// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class DefaultProfaneContentFilterServiceTests
{
    private const string Path = "CustomData/CustomWords.txt";

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private IProfaneContentFilterService _sut;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    private static readonly string[] s_webFormsArray = ["WebForms"];

    public DefaultProfaneContentFilterServiceTests() => _sut = new DefaultProfaneContentFilterService(
        cache: new MemoryCache(Options.Create<MemoryCacheOptions>(new())),
        logger: NullLogger<DefaultProfaneContentFilterService>.Instance);

    [TestInitialize]
    public void Initialize() => _sut = new DefaultProfaneContentFilterService(
        cache: new MemoryCache(Options.Create<MemoryCacheOptions>(new())),
        logger: NullLogger<DefaultProfaneContentFilterService>.Instance);

    [TestCleanup]
    public void Cleanup() => File.Delete(Path);

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow("This is a clean sentence.", "This is a clean sentence.")]
    [DataRow("This is a sentence with the word crap.", @"This is a sentence with the word \*\*\*\*.")]
    [DataRow("This is a sentence with the word CrAp.", @"This is a sentence with the word \*\*\*\*.")]
    [DataRow("This is a sentence with the word crap and shit.", @"This is a sentence with the word \*\*\*\* and \*\*\*\*.")]
    [DataRow("This is a sentence with the word crap and shit and fuck.", @"This is a sentence with the word \*\*\*\* and \*\*\*\* and \*\*\*\*.")]
    [DataRow("This is a sentence with the word crap and shit and fuck and ass.", @"This is a sentence with the word \*\*\*\* and \*\*\*\* and \*\*\*\* and \*\*\*.")]
    public async Task FilterProfanityAsyncReturnsExpectedResult(string? input, string? expectedResult)
    {
        // Act
        var result = await _sut.FilterProfanityAsync(input!,
            new(ReplacementStrategy.Asterisk, FilterTarget.Body));

        // Assert
        Assert.AreEqual(expectedResult, result.FinalOutput ?? input);

        if (result.IsFiltered)
        {
            Assert.IsTrue(result.Matches.Count > 0);
        }
        else
        {
            Assert.IsNull(result.Matches);
        }
    }

    [TestMethod]
    public async Task FilterProfanityAsyncMultipleParametersReturnsValidResults()
    {
        var input = "I love WebForms!";

        var set = new HashSet<ProfaneSourceFilter>
        {
            new("Custom", s_webFormsArray.ToFrozenSet())
        };

        // Act
        var titleResult = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.Bleep, FilterTarget.Title)
            {
                AdditionalFilterSources = set
            });

        var bodyResult = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.Emoji, FilterTarget.Body)
            {
                AdditionalFilterSources = set
            });

        // Assert
        Assert.AreEqual("I love bleep!", titleResult.FinalOutput);
        Assert.IsTrue(titleResult.IsFiltered);
        // CollectionAssert.That.Single(titleResult.Matches);

        Assert.AreNotEqual(input, bodyResult.FinalOutput);
        Assert.IsTrue(bodyResult.IsFiltered);
        //CollectionAssert.That.Single(bodyResult.Matches);
    }

    [TestMethod]
    public async Task FilterProfanityAsyncWithEmojiReturnsValidResult()
    {
        var input = "This is fucking bullshit!";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.Emoji, FilterTarget.Body));

        // Assert
        Assert.AreNotEqual(input, result.FinalOutput);
        Assert.IsTrue(result.IsFiltered);
        Assert.AreEqual(2, result.Matches.Count);
    }

    [TestMethod]
    public async Task FilterProfanityAsyncWithManualProfaneWordsReturnsExpectedResult()
    {
        var input = "Does this get filtered if I say WebForms?! Well, does it?";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body)
            {
                AdditionalFilterSources =
                [
                    new("ManualProfaneWords", s_webFormsArray.ToFrozenSet())
                ]
            });

        // Assert
        Assert.IsTrue(result.IsFiltered);
    }

    [TestMethod]
    public async Task FilterProfanityAsyncDoesNotReportFalseNegative()
    {
        var input = """
                      ## Purpose
            <!-- Describe the intention of the changes being proposed. What problem does it solve or functionality does it add? -->
            * ...

            ## Does this introduce a breaking change?
            <!-- Mark one with an "x". -->
            ```
            [ ] Yes
            [ ] No
            ```

            ## Pull Request Type
            What kind of change does this Pull Request introduce?

            <!-- Please check the one that applies to this PR using "x". -->
            ```
            [ ] Bugfix
            [ ] Feature
            [ ] Code style update (formatting, local variables)
            [ ] Refactoring (no functional changes, no api changes)
            [ ] Documentation content changes
            [ ] Other... Please describe:
            ```

            ## How to Test
            *  Get the code

            ```
            git clone [repo-address]
            cd [repo-name]
            git checkout [branch-name]
            npm install
            ```

            * Test the code
            <!-- Add steps to run the tests suite and/or manually test -->
            ```
            ```

            ## What to Check
            Verify that the following are valid
            * ...

            ## Other Information
            <!-- Add any other helpful information that may be needed here. -->
            """;

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body));

        // Assert
        Assert.IsFalse(result.IsFiltered);
    }

    [TestMethod]
    public async Task FilterProfanityAsyncWithMiddleAsteriskReturnsMultiStepResult()
    {
        var input = "Lots of fucking words like manky and arrusa!";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body));

        // Assert
        Assert.IsTrue(result.IsFiltered);

        Assert.AreEqual(@"Lots of f\*\*\*\*\*g words like m\*\*\*y and a\*\*\*\*a!", result.FinalOutput);

        Assert.IsTrue(result.Steps.Count is > 8);
        Assert.AreEqual(3, result.Steps.Count(static step => step.IsFiltered));

        CollectionAssert.That.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("GoogleBannedWords.txt", StringComparison.OrdinalIgnoreCase));
        CollectionAssert.That.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("BritishSwearWords.txt", StringComparison.OrdinalIgnoreCase));
        CollectionAssert.That.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("ItalianSwearWords.txt", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task FilterProfanityAsyncWithCustomDataReturnsMultiStepResult()
    {
        string[] customWords = ["Silverlight", "WebForms"];
        Directory.CreateDirectory("CustomData");

        await File.WriteAllTextAsync(Path, string.Join('\n', customWords));

        var input = "My least favorite web technologies are Silverlight and WebForms!";

        // Act
        var result = await _sut.FilterProfanityAsync(input,
            new(ReplacementStrategy.MiddleAsterisk, FilterTarget.Body));

        // Assert
        Assert.IsTrue(result.IsFiltered);

        Assert.AreEqual(@"My least favorite web technologies are S\*\*\*\*\*\*\*\*\*t and W\*\*\*\*\*\*s!", result.FinalOutput);

        Assert.AreEqual(10, result.Steps.Count);
        Assert.AreEqual(1, result.Steps.Count(static step => step.IsFiltered));

        CollectionAssert.That.Contains(result.Steps,
            static step => step.ProfaneSourceData.EndsWith("CustomWords.txt", StringComparison.OrdinalIgnoreCase));
    }
}