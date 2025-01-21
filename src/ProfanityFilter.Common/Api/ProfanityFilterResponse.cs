// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// A representation of a profanity-filter response object.
/// </summary>
/// <param name="ContainsProfanity">A boolean value indicating whether or
/// not the the <paramref name="InputText"/> contains profanity.</param>
/// <param name="InputText">The original input text.</param>
/// <param name="FilteredText">The final output text, after filters have been applied.</param>
/// <param name="ReplacementStrategy">The replacement strategy used.</param>
/// <param name="FiltrationSteps">An array of steps representing the
/// filtration process, step-by-step.</param>
/// <param name="Matches">An optional set of matches.</param>
public sealed record class ProfanityFilterResponse(
    [property:MemberNotNullWhen(
        true,
        nameof(ProfanityFilterResponse.FilteredText),
        nameof(ProfanityFilterResponse.FiltrationSteps),
        nameof(ProfanityFilterResponse.Matches))]
    bool ContainsProfanity,
    string InputText,
    string? FilteredText,
    ReplacementStrategy ReplacementStrategy,
    ProfanityFilterStep[]? FiltrationSteps = default,
    string[]? Matches = default)
{
    /// <summary>
    /// Creates a new instance of <see cref="ProfanityFilterResponse"/> from the given <see cref="FilterResult"/> and <see cref="ReplacementStrategy"/>.
    /// </summary>
    /// <param name="result">The filter result containing the filtration details.</param>
    /// <param name="strategy">The replacement strategy used during filtration.</param>
    /// <returns>A new instance of <see cref="ProfanityFilterResponse"/>.</returns>
    public static ProfanityFilterResponse From(
        FilterResult result, ReplacementStrategy strategy) =>
        new(
                ContainsProfanity: result.IsFiltered,
                InputText: result.Input ?? "",
                FilteredText: result.FinalOutput,
                ReplacementStrategy: strategy,
                FiltrationSteps: [.. result.Steps?.Where(static s => s.IsFiltered) ?? []],
                Matches: [.. result.Matches ?? []]
            );
}
