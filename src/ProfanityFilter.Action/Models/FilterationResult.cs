// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

/// <summary>
/// Represents a filtration result, with title and body results.
/// </summary>
/// <param name="TitleResult">The title result.</param>
/// <param name="BodyResult">The body result.</param>
internal readonly record struct FiltrationResult(
    FilterResult? TitleResult = null,
    FilterResult? BodyResult = null)
{
    /// <summary>
    /// Represents a non-filtered result.
    /// </summary>
    internal static FiltrationResult NotFiltered { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the tile or body is filtered.
    /// </summary>
    internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;

    /// <summary>
    /// Gets a value indicating whether the title is filtered.
    /// </summary>
    [MemberNotNullWhen(true, nameof(TitleResult), nameof(Title))]
    internal bool IsTitleFiltered => TitleResult?.IsFiltered == true;

    /// <summary>
    /// Gets a value indicating whether the body is filtered.
    /// </summary>
    [MemberNotNullWhen(true, nameof(BodyResult))]
    internal bool IsBodyFiltered => BodyResult?.IsFiltered == true;

    /// <summary>
    /// Gets the final output of the title.
    /// </summary>
    internal string Title => IsTitleFiltered
        ? TitleResult.FinalOutput ?? TitleResult.Input
        : "";

    /// <summary>
    /// Gets the final output of the body.
    /// </summary>
    internal string Body => IsBodyFiltered
        ? BodyResult.FinalOutput ?? BodyResult.Input
        : "";
}
