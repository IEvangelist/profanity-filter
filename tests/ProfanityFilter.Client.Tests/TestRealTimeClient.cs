// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Tests;

internal class TestRealTimeClient : IRealtimeClient
{
    IAsyncEnumerable<ProfanityFilterResponse> IRealtimeClient.StreamAsync(IAsyncEnumerable<ProfanityFilterRequest> liveRequests, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask IRealtimeClient.StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ValueTask IRealtimeClient.StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
