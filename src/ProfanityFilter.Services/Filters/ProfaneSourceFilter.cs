// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Represents a filter that is used to filter profane words from a source.
/// </summary>
/// <param name="SourceName">The name of the source.</param>
/// <param name="ProfaneWords">A set of profane words to filter on.</param>
public record class ProfaneSourceFilter(
    string SourceName,
    FrozenSet<string> ProfaneWords)
{
    /// <summary>
    /// Gets the string representation of the <see cref="ProfaneWords"/> set
    /// represented as a regular expression pattern.
    /// </summary>
    public string RegexPattern { get; } =
        $"\\b({string.Join('|', ProfaneWords)})\\b";
}
