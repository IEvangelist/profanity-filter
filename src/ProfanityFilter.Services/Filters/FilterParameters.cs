// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Filters;

/// <summary>
/// Represents the parameters used when replacing profane content.
/// </summary>
/// <param name="Strategy">The replacement strategy used when filtering.</param>
/// <param name="Target">The target of the filter.</param>
public readonly record struct FilterParameters(
    ReplacementStrategy Strategy,
    FilterTarget Target)
{
    /// <summary>
    /// A value used to uniquely identify the filter parameters.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();
}
