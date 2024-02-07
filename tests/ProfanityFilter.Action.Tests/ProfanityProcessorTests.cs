// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

public class ProfanityProcessorTests
{
    [Fact]
    public async Task ProfanityProcessorCorrectlyNoopsWithNullContextTest()
    {
        var sut = new ProfanityProcessor(
            new TestCustomGitHubClient(),
            new TestProfanityFilterService(),
            new TestCoreService([]));

        await sut.ProcessProfanityAsync(null);
    }
}
