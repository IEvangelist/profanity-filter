// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

internal readonly record struct FiltrationResult(
    FilterResult? TitleResult = null,
    FilterResult? BodyResult = null)
{
    internal static FiltrationResult NotFiltered { get; } = new();

    internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;

    [MemberNotNullWhen(true, nameof(TitleResult), nameof(Title))]
    internal bool IsTitleFiltered => TitleResult?.IsFiltered == true;

    [MemberNotNullWhen(true, nameof(BodyResult))]
    internal bool IsBodyFiltered => BodyResult?.IsFiltered == true;

    internal string Title => IsTitleFiltered
        ? TitleResult.FinalOutput ?? TitleResult.Input
        : "";

    internal string Body => IsBodyFiltered
        ? BodyResult.FinalOutput ?? BodyResult.Input
        : "";
}
