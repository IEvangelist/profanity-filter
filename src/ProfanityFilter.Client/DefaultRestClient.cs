// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

/// <inheritdoc cref="IRestClient" />
internal sealed class DefaultRestClient(HttpClient client) : IRestClient
{
    /// <inheritdoc />
    async Task<IMaybe<ProfanityFilterResponse>> IRestClient.ApplyFilterAsync(ProfanityFilterRequest request)
    {
        using var response = await client.PostAsJsonAsync(
            "filter", request, JsonSerializationContext.Default.ProfanityFilterRequest);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync(
            JsonSerializationContext.Default.ProfanityFilterResponse);

        return result.AsMaybe();
    }

    /// <inheritdoc />
    Task<IMaybe<string[]>> IRestClient.GetDataByNameAsync(string name)
    {
        return GetMaybeAsync($"data/{name}", JsonSerializationContext.Default.StringArray);
    }

    /// <inheritdoc />
    Task<IMaybe<string[]>> IRestClient.GetDataNamesAsync()
    {
        return GetMaybeAsync("data", JsonSerializationContext.Default.StringArray);
    }

    /// <inheritdoc />
    Task<IMaybe<StrategyResponse[]>> IRestClient.GetStrategiesAsync()
    {
        return GetMaybeAsync("strategies", JsonSerializationContext.Default.StrategyResponseArray);
    }

    /// <inheritdoc />
    Task<IMaybe<FilterTargetResponse[]>> IRestClient.GetTargetsAsync()
    {
        return GetMaybeAsync("targets", JsonSerializationContext.Default.FilterTargetResponseArray);
    }

    /// <inheritdoc />
    async Task<IMaybe<T>> GetMaybeAsync<T>(string uri, JsonTypeInfo<T> typeInfo)
    {
        var data = await client.GetFromJsonAsync(uri, typeInfo);

        return data.AsMaybe();
    }
}
