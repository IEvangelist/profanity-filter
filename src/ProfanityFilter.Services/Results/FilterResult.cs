// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Results;

/// <summary>
/// Represents the result of a filter operation.
/// </summary>
/// <param name="Input">The value to check for profane content.</param>
/// <param name="Parameters">The parameters used to generate this filter result.</param>
/// <param name="Steps">The steps for the filter result involved in the filter process.</param>
/// <param name="Matches">The list of matches that were found in the filter process.</param>
public record class FilterResult(
    string Input,
    FilterParameters Parameters,
    List<FilterStep>? Steps = null,
    List<string>? Matches = null)
{
    /// <summary>
    /// Gets a value indicating whether the input is filtered.
    /// </summary>
    [MemberNotNullWhen(
        returnValue: true,
        nameof(Steps), nameof(FinalOutput), nameof(Matches))]
    public bool IsFiltered
        => FinalOutput is not null;

    /// <summary>
    /// Gets the final result of a filter operation. If the input is not
    /// filtered, <see langword="null"/> is returned.
    /// </summary>
    public string? FinalOutput
        => Steps?.LastOrDefault(static step => step.IsFiltered)?.Output;
}
