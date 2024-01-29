// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Results;

/// <summary>
/// Represents the result of a censor operation.
/// </summary>
/// <param name="Input">The value to check for profane content.</param>
/// <param name="Steps">The steps fo</param>
public record class CensorResult(
    string Input,
    List<CensorStep>? Steps = null)
{
    /// <summary>
    /// Gets a value indicating whether the input is censored.
    /// </summary>
    [MemberNotNullWhen(true, nameof(FinalOutput), nameof(Steps))]
    public bool IsCensored
        => FinalOutput is not null;

    /// <summary>
    /// Gets the result of the censor filter applied. If the input is not
    /// censored, <see langword="null"/> is returned."
    /// </summary>
    public string? FinalOutput
        => Steps?.LastOrDefault(static step => step.IsCensored)?.Output;
}
