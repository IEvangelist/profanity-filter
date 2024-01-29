// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class CensorResultTests
{
    [Fact]
    public void CensorResultCorrectlyReportsIsCensored()
    {
        CensorResult sut = new("Fake input");

        Assert.False(sut.IsCensored);
        Assert.Equal("Fake input", sut.Input);
        Assert.Null(sut.FinalOutput);

        sut = sut with
        {
            Steps =
            [
                new CensorStep("Fake input", "bad-words.txt", "Fake ****")
            ]
        };

        Assert.True(sut.IsCensored);
        Assert.Equal("Fake ****", sut.FinalOutput);
    }
}
