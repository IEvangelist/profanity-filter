// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

internal sealed class DefaultRealtimeClient(
    IConfiguration configuration,
    IOptions<ProfanityFilterOptions> options,
    ILogger<DefaultRealtimeClient> logger) : IRealtimeClient
{
    private readonly Uri _hubUrl = new UriBuilder(
        options.Value.ApiBaseAddress ?? throw new ArgumentException("""
            The API base address must be provided.
            """))
    {
        Path = "profanity/hub"
    }.Uri;

    private HubConnection? _connection = null;

    [MemberNotNull(nameof(_connection))]
    private void EnsureInitialized()
    {
        _connection ??= new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                // Only apply these options when running in a container.
                if (!configuration.IsRunningInContainer())
                {
                    return;
                }

                options.UseDefaultCredentials = true;
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is HttpClientHandler clientHandler)
                    {
                        clientHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }

                    return handler;
                };
            })
            .WithAutomaticReconnect()
            .WithStatefulReconnect()
            .Build();
    }

    /// <inheritdoc />
    async ValueTask IRealtimeClient.StartAsync(CancellationToken cancellationToken)
    {
        EnsureInitialized();

        _connection.Closed += OnHubConnectionClosedAsync;
        _connection.Reconnected += OnHubConnectionReconnectedAsync;
        _connection.Reconnecting += OnHubConnectionReconnectingAsync;

        await _connection.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    async ValueTask IRealtimeClient.StopAsync(CancellationToken cancellationToken)
    {
        EnsureInitialized();

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
    bool IRealtimeClient.IsConnected => _connection is
    {
        State: HubConnectionState.Connected
    };

    /// <inheritdoc />
    IAsyncEnumerable<ProfanityFilterResponse> IRealtimeClient.StreamAsync(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        CancellationToken cancellationToken)
    {
        EnsureInitialized();

        return _connection.StreamAsync<ProfanityFilterResponse>(
            methodName: "live",
            arg1: liveRequests,
            cancellationToken: cancellationToken);
    }
}