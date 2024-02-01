// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

internal readonly record struct FiltrationResult(
    string Title,
    bool IsTitleFiltered,
    string Body,
    bool IsBodyFiltered)
{
    internal static FiltrationResult NotFiltered { get; } =
        new(string.Empty, false, string.Empty, false);

    internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;
}
