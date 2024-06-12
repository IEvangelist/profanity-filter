// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Models;

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
    string[]? Matches = default);
