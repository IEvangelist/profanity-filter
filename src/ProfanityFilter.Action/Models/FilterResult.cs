// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

internal readonly record struct FilterResult(
    string Title,
    bool IsTitleFiltered,
    string Body,
    bool IsBodyFiltered)
{
    internal static FilterResult NotFiltered { get; } =
        new(string.Empty, false, string.Empty, false);

    internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;
}
