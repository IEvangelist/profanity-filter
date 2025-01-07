// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Tests;

internal class TestRestClient : IRestClient
{
    Task<IMaybe<ProfanityFilterResponse>> IRestClient.ApplyFilterAsync(ProfanityFilterRequest request)
    {
        throw new NotImplementedException();
    }

    Task<IMaybe<string[]>> IRestClient.GetDataByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    Task<IMaybe<string[]>> IRestClient.GetDataNamesAsync()
    {
        throw new NotImplementedException();
    }

    Task<IMaybe<StrategyResponse[]>> IRestClient.GetStrategiesAsync()
    {
        throw new NotImplementedException();
    }

    Task<IMaybe<FilterTargetResponse[]>> IRestClient.GetTargetsAsync()
    {
        throw new NotImplementedException();
    }
}
