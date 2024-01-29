// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Results;

/// <summary>
/// Represents a step in the censoring process.
/// </summary>
/// <param name="Input">The input for the step.</param>
/// <param name="ProfaneSourceData">The profane source data for the step.</param>
/// <param name="Output">The output of the step.</param>
public record class CensorStep(
    string Input,
    string ProfaneSourceData,
    string? Output = null)
{
    /// <summary>
    /// A value indicating whether the step is censored.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Output))]
    public bool IsCensored
        => Output is not null;
}