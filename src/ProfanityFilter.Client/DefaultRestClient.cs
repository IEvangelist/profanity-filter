// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;

/// <inheritdoc cref="IRestClient" />
internal sealed class DefaultRestClient(HttpClient client) : IRestClient
{
    /// <inheritdoc />
    async Task<Maybe<ProfanityFilterResponse>> IRestClient.ApplyFilterAsync(ProfanityFilterRequest request)
    {
        using var response = await client.PostAsJsonAsync(
            "profanity/filter", request, JsonSerializationContext.Default.ProfanityFilterRequest);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync(
            JsonSerializationContext.Default.ProfanityFilterResponse);

        return result.AsMaybe();
    }

    /// <inheritdoc />
    Task<Maybe<string[]>> IRestClient.GetDataByNameAsync(string name) => GetMaybeAsync($"profanity/data/{name}", JsonSerializationContext.Default.StringArray);

    /// <inheritdoc />
    Task<Maybe<string[]>> IRestClient.GetDataNamesAsync() => GetMaybeAsync("profanity/data", JsonSerializationContext.Default.StringArray);

    /// <inheritdoc />
    Task<Maybe<StrategyResponse[]>> IRestClient.GetStrategiesAsync() => GetMaybeAsync("profanity/strategies", JsonSerializationContext.Default.StrategyResponseArray);

    /// <inheritdoc />
    Task<Maybe<FilterTargetResponse[]>> IRestClient.GetTargetsAsync() => GetMaybeAsync("profanity/targets", JsonSerializationContext.Default.FilterTargetResponseArray);

    /// <inheritdoc />
    private async Task<Maybe<T>> GetMaybeAsync<T>(string uri, JsonTypeInfo<T> typeInfo)
    {
        var data = await client.GetFromJsonAsync<T>(uri, typeInfo);

        return data.AsMaybe();
    }
}
