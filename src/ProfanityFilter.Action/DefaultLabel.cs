// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

/// <summary>
/// Provides default label information for profane content.
/// </summary>
internal static class DefaultLabel
{
    /// <summary>
    /// The default name of the label that is applied to content that is deemed to be profane.
    /// <c>"profane content 🤬"</c>
    /// </summary>
    internal const string Name = "profane content 🤬";

    /// <summary>
    /// A default label description indicating that either the title or body text contained profanity.
    /// </summary>
    internal const string Description = "Either the title or body text contained profanity";

    /// <summary>
    /// The default color for labels in the application, represented as a hexadecimal string.
    /// <c>#512bd4</c>
    /// </summary>
    internal const string Color = "512bd4";
}
