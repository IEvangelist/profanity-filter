// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common;

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

    /// <summary>
    /// A collection of additional profane words to consider as a filter source.
    /// </summary>
    public HashSet<ProfaneSourceFilter>? AdditionalFilterSources { get; init; }

    /// <summary>
    /// A collection of source names to exclude from filtering.
    /// Source name values can be either full file names or source aliases.
    /// </summary>
    public HashSet<string>? ExcludedFilterSources { get; init; }

    /// <summary>
    /// A collection of words to exclude from filtering, acting as a whitelist.
    /// </summary>
    public HashSet<string>? ExcludedWords { get; init; }
}
