// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

internal static class ReplacementStrategyExtensions
{
    /// <summary>
    /// Gets the corresponding function, that when given <see cref="FilterParameters"/>
    /// returns a <see cref="MatchEvaluator"/>.
    /// </summary>
    internal static Func<FilterParameters, MatchEvaluator> GetMatchEvaluator(this ReplacementStrategy replacementStrategy)
    {
        return replacementStrategy switch
        {
            ReplacementStrategy.Emoji => MatchEvaluators.EmojiEvaluator,
            ReplacementStrategy.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementStrategy.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementStrategy.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementStrategy.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,
            ReplacementStrategy.FirstLetterThenAsterisk => MatchEvaluators.FirstLetterThenAsteriskEvaluator,
            ReplacementStrategy.AngerEmoji => MatchEvaluators.AngerEmojiEvaluator,
            ReplacementStrategy.Bleep => MatchEvaluators.BleepEvaluator,
            ReplacementStrategy.RedactedRectangle => MatchEvaluators.RedactedRectangleEvaluator,
            ReplacementStrategy.StrikeThrough => MatchEvaluators.StrikeThroughEvaluator,
            ReplacementStrategy.Underscores => MatchEvaluators.UnderscoresEvaluator,
            ReplacementStrategy.Grawlix => MatchEvaluators.GrawlixEvaluator,
            ReplacementStrategy.BoldGrawlix => MatchEvaluators.BoldGrawlixEvaluator,

            _ => MatchEvaluators.AsteriskEvaluator,
        };
    }
}
