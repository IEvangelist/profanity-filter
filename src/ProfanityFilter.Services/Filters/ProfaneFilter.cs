// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal record class ProfaneFilter(
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
