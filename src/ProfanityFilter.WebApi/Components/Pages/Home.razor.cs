// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Components.Pages;

/// <summary>
/// The home page.
/// </summary>
[StreamRendering]
public sealed partial class Home(
    ILogger<Home> logger,
    IRealtimeClient realtimeClient,
    ILocalStorageService localStorage) : IAsyncDisposable
{
    private readonly SystemTimer _debounceTimer = new()
    {
        Interval = 500,
        AutoReset = false
    };

    private readonly ReplacementStrategy[] _strategies = Enum.GetValues<ReplacementStrategy>();
    private readonly CancellationTokenSource _cts = new();

    private IObservable<ProfanityFilterRequest>? _liveRequests;
    private ReplacementStrategy _selectedStrategy;
    private string? _text = "Content to filter...";
    private bool _isLoading;
    private bool _isActive;

    /// <inheritdoc cref="ComponentBase.OnAfterRenderAsync"/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender is false)
        {
            return;
        }

        if (await localStorage.GetItemAsync<string>("selected-strategy") is { } strategy &&
            Enum.TryParse<ReplacementStrategy>(strategy, out var selectedStrategy))
        {
            _selectedStrategy = selectedStrategy;
            StateHasChanged();
        }

        await realtimeClient.StartAsync();

        logger.HubStarted();

        _ = StartLiveUpdateStreamAsync();
    }

    private async Task StartLiveUpdateStreamAsync()
    {
        if (!realtimeClient.IsConnected || _liveRequests is null)
        {
            return;
        }

        while (true)
        {
            try
            {
                _liveRequests ??= Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
                        handler => _debounceTimer.Elapsed += handler,
                        handler => _debounceTimer.Elapsed -= handler
                    )
                    .Select(args =>
                    {
                        _isLoading = true;

                        return new ProfanityFilterRequest(
                            _text ?? "",
                            _selectedStrategy,
                            0
                        );
                    });

                // Producer requests...
                var liveRequests = _liveRequests.ToAsyncEnumerable();

                // Consumer responses...
                await foreach (var response in realtimeClient.StreamAsync(liveRequests, cancellationToken: _cts.Token))
                {
                    if (response is null)
                    {
                        break;
                    }

                    if (response is { FilteredText.Length: > 0 })
                    {
                        _text = response.FilteredText;
                    }

                    _isLoading = false;

                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex) when (Debugger.IsAttached && ex is not OperationCanceledException)
            {
                logger.ErrorProcessProfanity(ex);

                _ = ex;

                Debugger.Break();

                throw;
            }
            finally
            {
                _isLoading = false;
            }
        }
    }

    private void OnTextChanged(ChangeEventArgs args)
    {
        _text = args?.Value?.ToString();

        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private async Task OnSelectedStrategyChanged(ReplacementStrategy strategy)
    {
        _selectedStrategy = strategy;
        _isActive = false;

        await localStorage.SetItemAsync("selected-strategy", strategy.ToString());
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        _debounceTimer.Stop();
        _debounceTimer.Dispose();

        await _cts.CancelAsync();
        _cts.Dispose();

        if (realtimeClient is not null)
        {
            await realtimeClient.StopAsync();
        }
    }
}
