// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


namespace ProfanityFilter.Services.Extensions;

internal static class ReplacementStrategyExtensions
{
    internal static MatchEvaluator ToMatchEvaluator(this ReplacementStrategy replacementStrategy)
    {
        return replacementStrategy switch
        {
            ReplacementStrategy.Asterisk => MatchEvaluators.AsteriskEvaluator,
            ReplacementStrategy.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementStrategy.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementStrategy.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementStrategy.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,
            ReplacementStrategy.AngerEmoji => MatchEvaluators.AngerEmojiEvaluator,
            ReplacementStrategy.Bleep => MatchEvaluators.BleepEvaluator,
            ReplacementStrategy.RedactedBlackRectangle => MatchEvaluators.RedactedBlackRectangleEvaluator,
            ReplacementStrategy.StrikeThrough => MatchEvaluators.StrikeThroughEvaluator,
            ReplacementStrategy.Underscores => MatchEvaluators.UnderscoresEvaluator,

            _ => MatchEvaluators.EmojiEvaluator,
        };
    }
}
