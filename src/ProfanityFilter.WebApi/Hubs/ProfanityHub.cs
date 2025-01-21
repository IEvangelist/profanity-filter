// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Hubs;

internal sealed class ProfanityHub(
    IProfaneContentFilterService filter,
    ILogger<ProfanityHub> logger) : Hub
{
    [HubMethodName("live")]
    public async IAsyncEnumerable<ProfanityFilterResponse> LiveStream(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope(
            "ProfanityHub handling live stream...");

        logger.LogStartedStream(Context.ConnectionId);

        await foreach (var request in liveRequests.WithCancellation(cancellationToken))
        {
            var parameters = new FilterParameters(
                request.Strategy, request.Target);

            var result = await filter.FilterProfanityAsync(
                request.Text, parameters, cancellationToken);

            if (result is null or { FinalOutput: null })
            {
                logger.LogInvalidFilterResult(Context.ConnectionId);

                yield break;
            }

            yield return ProfanityFilterResponse.From(result, request.Strategy);
        }

        logger.LogEndingStream(Context.ConnectionId);
    }
}
