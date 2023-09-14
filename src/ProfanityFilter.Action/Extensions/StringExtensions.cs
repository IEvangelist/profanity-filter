// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class StringExtensions
{
    /// <summary>
    /// Converts the specified string value to a GitHub ID.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>A new instance of the <see cref="ID"/> class representing the GitHub ID.</returns>
    internal static ID ToGitHubId(this string value) => new(value);
}