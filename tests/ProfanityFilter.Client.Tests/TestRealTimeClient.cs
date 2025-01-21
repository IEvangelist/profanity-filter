// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Tests;

internal sealed class TestRealTimeClient : IRealtimeClient
{
    bool IRealtimeClient.IsConnected { get; }

    IAsyncEnumerable<ProfanityFilterResponse> IRealtimeClient.StreamAsync(IAsyncEnumerable<ProfanityFilterRequest> liveRequests, CancellationToken cancellationToken) => throw new NotImplementedException();

    ValueTask IRealtimeClient.StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    ValueTask IRealtimeClient.StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
