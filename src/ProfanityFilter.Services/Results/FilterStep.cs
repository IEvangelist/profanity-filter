// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Results;

/// <summary>
/// Represents a step in the profanity filtering process.
/// </summary>
/// <param name="Input">The input for the step. For example, <c>"Text with shitty words"</c>.</param>
/// <param name="ProfaneSourceData">The profane source data for the step. For example, <i>Data/SwearWordList.txt</i>.</param>
/// <param name="Output">The output of the step. For example, <c>"Text with 🤬 words"</c></param>
public record class FilterStep(
    string Input,
    string ProfaneSourceData,
    string? Output = null)
{
    /// <summary>
    /// Gets a value indicating whether the step is filtered.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Output))]
    public bool IsFiltered
        => Output is not null;
}