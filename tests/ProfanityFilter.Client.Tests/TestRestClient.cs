// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Tests;

internal sealed class TestRestClient : IRestClient
{
    Task<Maybe<ProfanityFilterResponse>> IRestClient.ApplyFilterAsync(ProfanityFilterRequest request) => throw new NotImplementedException();

    Task<Maybe<string[]>> IRestClient.GetDataByNameAsync(string name) => throw new NotImplementedException();

    Task<Maybe<string[]>> IRestClient.GetDataNamesAsync() => throw new NotImplementedException();

    Task<Maybe<StrategyResponse[]>> IRestClient.GetStrategiesAsync() => throw new NotImplementedException();

    Task<Maybe<FilterTargetResponse[]>> IRestClient.GetTargetsAsync() => throw new NotImplementedException();
}
