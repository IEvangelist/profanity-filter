// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

internal sealed class TestProfanityFilterService : IProfaneContentFilterService
{
    public ValueTask<FilterResult> FilterProfanityAsync(string content, FilterParameters parameters, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, ProfaneSourceFilter>> ReadAllProfaneWordsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
