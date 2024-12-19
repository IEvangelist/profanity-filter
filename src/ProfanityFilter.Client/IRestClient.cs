// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client;


/// <summary>
/// Defines the contract for a REST client that interacts with the profanity filter service.
/// </summary>
public interface IRestClient
{
    /// <summary>
    /// Applies the profanity filter to the given request.
    /// </summary>
    /// <param name="request">The request containing the text to be filtered.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IMaybe{T}"/> of <see cref="ProfanityFilterResponse"/>.</returns>
    Task<IMaybe<ProfanityFilterResponse>> ApplyFilterAsync(ProfanityFilterRequest request);

    /// <summary>
    /// Retrieves the available filtering strategies.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IMaybe{T}"/> of an array of <see cref="StrategyResponse"/>.</returns>
    Task<IMaybe<StrategyResponse[]>> GetStrategiesAsync();

    /// <summary>
    /// Retrieves the available filter targets.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IMaybe{T}"/> of an array of <see cref="FilterTargetResponse"/>.</returns>
    Task<IMaybe<FilterTargetResponse[]>> GetTargetsAsync();

    /// <summary>
    /// Retrieves the names of the data sets available for filtering.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IMaybe{T}"/> of an array of strings representing the data set names.</returns>
    Task<IMaybe<string[]>> GetDataNamesAsync();

    /// <summary>
    /// Retrieves the data set by its name.
    /// </summary>
    /// <param name="name">The name of the data set to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IMaybe{T}"/> of an array of strings representing the data.</returns>
    Task<IMaybe<string[]>> GetDataByNameAsync(string name);
}
