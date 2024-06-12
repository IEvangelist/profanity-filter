// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Models;

/// <summary>
/// A representation of a strategy response object.
/// </summary>
/// <param name="StrategyName">The name of the strategy.</param>
/// <param name="StrategyValue">The <c>int</c> value of the strategy.</param>
/// <param name="Description">The description of the strategy.</param>
public sealed record class StrategyResponse(
    string StrategyName,
    int StrategyValue,
    string Description)
{
    public static implicit operator StrategyResponse(ReplacementStrategy strategy)
    {
        var description = strategy switch
        {
            ReplacementStrategy.Asterisk => "Replaces the profanity word with asterisk.",
            ReplacementStrategy.Emoji => "Replaces the profanity word with an emoji.",
            ReplacementStrategy.AngerEmoji => "Replaces the profanity word with the one of the anger emoji.",
            ReplacementStrategy.MiddleSwearEmoji => "Represents a replacement strategy where the middle of the swear word is replaced with an emoji.",
            ReplacementStrategy.RandomAsterisk => "Represents a replacement strategy where the profanity is replaced with a random number of asterisk.",
            ReplacementStrategy.MiddleAsterisk => "Represents the middle asterisk replacement strategy, which replaces the characters in the middle of the profanity with asterisk.",
            ReplacementStrategy.FirstLetterThenAsterisk => "Represents the first letter then asterisk replacement strategy, which replaces everything after the first letter of the profanity with asterisk.",
            ReplacementStrategy.VowelAsterisk => "Represents a replacement strategy where vowels in a profane word are replaced with asterisk.",
            ReplacementStrategy.Bleep => "Represents a replacement strategy where the profanity is replaced with the word \"bleep\".",
            ReplacementStrategy.RedactedRectangle => "Represents a replacement strategy where the profane word has each letter replaced with the rectangle symbol <c>█</c>.",
            ReplacementStrategy.StrikeThrough => "Represents a replacement strategy where the profane word is <c>~~struck through~~</c>.",
            ReplacementStrategy.Underscores => "Represents a replacement strategy where the profane word is replaced by underscores.",
            ReplacementStrategy.Grawlix => "Represents a replacement strategy where the profane word is replaced by grawlix, for example <c>\"#$@!\"</c>.",
            ReplacementStrategy.BoldGrawlix => "Represents a replacement strategy where the profane word is replaced by bold grawlix, for example <c>\"#$@!\"</c>.",
            _ => "",
        };

        return new StrategyResponse(
            StrategyName: Enum.GetName(strategy) ?? strategy.ToString(),
            StrategyValue: (int)strategy,
            Description: description);
    }
}
