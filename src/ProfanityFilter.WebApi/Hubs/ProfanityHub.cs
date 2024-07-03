// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Hubs;

public sealed class ProfanityHub(
    IProfaneContentFilterService filter,
    ILogger<ProfanityHub> logger) : Hub
{
    [HubMethodName("live")]
    public async IAsyncEnumerable<ProfanityFilterResponse> LiveStream(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        logger.LogInformation("""
            Starting a live stream for: {ConnectionId}.
            """,
            Context.ConnectionId);

        await foreach (var request in liveRequests.WithCancellation(cancellationToken))
        {
            var parameters = new FilterParameters(
                request.Strategy, request.Target);

            var result = await filter.FilterProfanityAsync(
                request.Text, parameters, cancellationToken);

            if (result is null or { FinalOutput: null })
            {
                logger.LogWarning("""
                    ({ConnectionId}) Filter result was either null or its final output was null.
                    """,
                    Context.ConnectionId);

                yield break;
            }

            yield return ProfanityFilterResponse.From(result, request.Strategy);
        }

        logger.LogInformation("""
            Ending live stream for: {ConnectionId}.
            """,
            Context.ConnectionId);
    }
}
