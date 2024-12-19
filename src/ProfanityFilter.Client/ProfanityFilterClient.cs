// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

/// <summary>
/// A client for interacting with the profanity filter service.
/// </summary>
/// <param name="restClient">The REST client used for making HTTP requests.</param>
/// <param name="realtimeClient">The realtime client used for live streaming requests.</param>
public sealed class ProfanityFilterClient(IRestClient restClient, IRealtimeClient realtimeClient)
{
    /// <summary>
    /// Gets the REST client used for making HTTP requests.
    /// </summary>
    public IRestClient Rest { get; } = restClient;

    /// <summary>
    /// Gets the realtime client used for live streaming requests.
    /// </summary>
    public IRealtimeClient Realtime { get; } = realtimeClient;

    /// <summary>
    /// Deconstructs the <see cref="ProfanityFilterClient"/> into its REST and real-time clients.
    /// </summary>
    /// <param name="rest">The <see cref="IRestClient"/> implementation.</param>
    /// <param name="realtime">The <see cref="IRealtimeClient"/> implementation.</param>
    public void Deconstruct(out IRestClient rest, out IRealtimeClient realtime) =>
        (rest, realtime) = (Rest, Realtime);
}
