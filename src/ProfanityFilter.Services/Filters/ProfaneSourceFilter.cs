// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Represents a filter source that is used as an additive to the existing profane definitions.
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
    [JsonIgnore]
    public string RegexPattern { get; } =
        $"\\b({string.Join('|', ProfaneWords)})\\b";
}
