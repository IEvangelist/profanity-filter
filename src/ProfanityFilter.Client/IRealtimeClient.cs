// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

/// <summary>
/// Interface for a real-time client that handles profanity filtering.
/// </summary>
public interface IRealtimeClient
{
    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Starts the real-time client asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    ValueTask StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the real-time client asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    ValueTask StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams profanity filter responses asynchronously based on live requests.
    /// </summary>
    /// <param name="liveRequests">An asynchronous stream of profanity filter requests.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An asynchronous stream of profanity filter responses.</returns>
    IAsyncEnumerable<ProfanityFilterResponse> StreamAsync(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        CancellationToken cancellationToken);
}
