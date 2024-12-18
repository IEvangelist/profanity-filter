// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class FilterResultTests
{
    [TestMethod]
    public void CensorResultCorrectlyReportsIsCensored()
    {
        FilterResult sut = new(
            Input: "Fake input",
            Parameters: new(
                Strategy: ReplacementStrategy.AngerEmoji, Target: FilterTarget.Title));

        Assert.IsFalse(sut.IsFiltered);
        Assert.AreEqual("Fake input", sut.Input);
        Assert.IsNull(sut.FinalOutput);

        sut = sut with
        {
            Steps =
            [
                new FilterStep("Fake input", "bad-words.txt", "Fake ****")
            ]
        };

        Assert.IsTrue(sut.IsFiltered);
        Assert.AreEqual("Fake ****", sut.FinalOutput);
    }
}
