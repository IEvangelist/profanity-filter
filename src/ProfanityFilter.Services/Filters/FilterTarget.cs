// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Represents the filter target, meaning whether the filter is
/// being applied to a title, body, or comment.
/// </summary>
public enum FilterTarget
{
    /// <summary>
    /// Used to indicate that the target of the filter
    /// is the title of the issue or pull request.
    /// </summary>
    Title,

    /// <summary>
    /// Used to indicate that the target of the filter
    /// is the body of the issue or pull request.
    /// </summary>
    Body,

    /// <summary>
    /// Used to indicate that the target of the filter
    /// is a comment on an issue or pull request.
    /// </summary>
    Comment
}
