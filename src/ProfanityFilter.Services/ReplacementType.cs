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

    /// <summary>
    /// Represents a replacement type where the profanity is replaced 
    /// with a random number of asterisks.
    /// </summary>
    RandomAsterisk,

    /// <summary>
    /// Represents the middle asterisk replacement type, which replaces the 
    /// characters in the middle of the profanity with asterisks.
    /// </summary>
    MiddleAsterisk,

    /// <summary>
    /// Represents a replacement type where the middle of the swear word 
    /// is replaced with an emoji.
    /// </summary>
    MiddleSwearEmoji,

    /// <summary>
    /// Represents a replacement type where vowels in a profane word
    /// are replaced with asterisks.
    /// </summary>
    VowelAsterisk,
}
