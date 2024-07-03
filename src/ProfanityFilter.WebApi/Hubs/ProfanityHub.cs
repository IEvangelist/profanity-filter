// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Hubs;

public sealed class ProfanityHub(IProfaneContentFilterService filter) : Hub
{
    [HubMethodName("live")]
    public async IAsyncEnumerable<ProfanityFilterResponse> LiveStream(
        IAsyncEnumerable<ProfanityFilterRequest> liveRequests,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        await foreach (var request in liveRequests)
        {
            var parameters = new FilterParameters(
                request.Strategy, request.Target);

            var result = await filter.FilterProfanityAsync(
                request.Text, parameters, cancellationToken);

            if (result is null or { FinalOutput: null })
            {
                yield break;
            }

            yield return (result, request.Strategy);
        }
    }
}
