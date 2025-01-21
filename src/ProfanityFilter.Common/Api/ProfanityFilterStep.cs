// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// A representation of a profanity-filter step, detailing
/// each step applied to a filter operation.
/// </summary>
/// <param name="Input">The input before the step ran.</param>
/// <param name="Output">The output after the step ran.</param>
/// <param name="ProfaneSourceData">The profane source data for the step. For example, <i>Data/SwearWordList.txt</i>.</param>
public sealed record class ProfanityFilterStep(
    string Input,
    string Output,
    string ProfaneSourceData)
{
    /// <summary>
    /// Converts a <see cref="FilterStep"/> to a <see cref="ProfanityFilterStep"/>.
    /// </summary>
    /// <param name="step">The filter step to convert.</param>
    /// <returns>A new <see cref="ProfanityFilterStep"/> instance.</returns>
    public static implicit operator ProfanityFilterStep(FilterStep step) =>
        new(step.Input, step.Output!, step.ProfaneSourceData);
}
