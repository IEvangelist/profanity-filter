// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Specifies the strategy to use when replacing a profane word.
/// </summary>
public enum ReplacementStrategy
{
    /// <summary>
    /// Replaces the profanity word with asterisk.
    /// </summary>
    Asterisk,

    /// <summary>
    /// Replaces the profanity word with an emoji.
    /// </summary>
    Emoji,

    /// <summary>
    /// Replaces the profanity word with the one of the anger emoji.
    /// </summary>
    AngerEmoji,

    /// <summary>
    /// Represents a replacement strategy where the middle of the swear word 
    /// is replaced with an emoji.
    /// </summary>
    MiddleSwearEmoji,

    /// <summary>
    /// Represents a replacement strategy where the profanity is replaced 
    /// with a random number of asterisk.
    /// </summary>
    RandomAsterisk,

    /// <summary>
    /// Represents the middle asterisk replacement strategy, which replaces the 
    /// characters in the middle of the profanity with asterisk.
    /// </summary>
    MiddleAsterisk,

    /// <summary>
    /// Represents the first letter then asterisk replacement strategy, which replaces
    /// everything after the first letter of the profanity with asterisk.
    /// </summary>
    FirstLetterThenAsterisk,

    /// <summary>
    /// Represents a replacement strategy where vowels in a profane word
    /// are replaced with asterisk.
    /// </summary>
    VowelAsterisk,

    /// <summary>
    /// Represents a replacement strategy where the profanity is replaced with a
    /// the word "bleep".
    /// </summary>
    Bleep,

    /// <summary>
    /// Represents a replacement strategy where the profane word has each letter
    /// replaced with the rectangle symbol <c>█</c>.
    /// </summary>
    RedactedRectangle,

    /// <summary>
    /// Represents a replacement strategy where the profane word is <c>~~struck through~~</c>.
    /// </summary>
    StrikeThrough,

    /// <summary>
    /// Represents a replacement strategy where the profane word is replaced by underscores.
    /// </summary>
    Underscores,

    /// <summary>
    /// Represents a replacement strategy where the profane word is replaced by grawlix, for example <c>"#$@!"</c>.
    /// </summary>
    Grawlix,

    /// <summary>
    /// Represents a replacement strategy where the profane word is replaced by bold grawlix, for example <c>"#$@!"</c>.
    /// </summary>
    BoldGrawlix
}
