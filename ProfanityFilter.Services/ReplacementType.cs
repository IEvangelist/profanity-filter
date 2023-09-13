// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Specifies the type of replacement to use for a profanity word.
/// </summary>
public enum ReplacementType
{
    /// <summary>
    /// Replaces the profanity word with asterisks.
    /// </summary>
    Asterisk,
    
    /// <summary>
    /// Replaces the profanity word with an emoji.
    /// </summary>
    Emoji,

    // TODO: Consider additional replacement types/match evaluator pairs...
}
