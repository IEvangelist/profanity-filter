// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

internal sealed class DefaultRealtimeClient(
    IOptions<ProfanityFilterOptions> options,
    ILogger<DefaultRealtimeClient> logger) : IRealtimeClient
{
    private readonly HubConnection _connection = new HubConnectionBuilder()
        .WithUrl(options.Value.RealtimeUri)
        .WithAutomaticReconnect()
        .WithStatefulReconnect()
        .Build();

    /// <inheritdoc />
    async ValueTask IRealtimeClient.StartAsync(CancellationToken cancellationToken)
    {
        _connection.Closed += OnHubConnectionClosedAsync;
        _connection.Reconnected += OnHubConnectionReconnectedAsync;
        _connection.Reconnecting += OnHubConnectionReconnectingAsync;

        await _connection.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    async ValueTask IRealtimeClient.StopAsync(CancellationToken cancellationToken)
    {
        _connection.Closed -= OnHubConnectionClosedAsync;
        _connection.Reconnected -= OnHubConnectionReconnectedAsync;
        _connection.Reconnecting -= OnHubConnectionReconnectingAsync;

        await _connection.StopAsync(cancellationToken);
    }

    private Task OnHubConnectionClosedAsync(Exception? exception)
    {
        logger.HubConnectionClosed(exception);

        return Task.CompletedTask;
    }

    private Task OnHubConnectionReconnectingAsync(Exception? exception)
    {
        logger.HubReconnecting(exception);

        return Task.CompletedTask;
    }

    private Task OnHubConnectionReconnectedAsync(string? arg)
    {
        logger.HubReconnected(arg);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    async IAsyncEnumerable<ProfanityFilterResponse> IRealtimeClient.LiveStreamAsync(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<ProfanityFilterResponse>();

        _connection.On<ProfanityFilterResponse>(
            "live", response => channel.Writer.TryWrite(response));

        var sendTask = Task.Run(async () =>
        {
            await foreach (var request in liveRequests.WithCancellation(cancellationToken))
            {
                await _connection.SendAsync("LiveStream", request, cancellationToken);
            }

            channel.Writer.Complete();
        },
        cancellationToken);

        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var response))
            {
                yield return response;
            }
        }

        await sendTask;
    }
}