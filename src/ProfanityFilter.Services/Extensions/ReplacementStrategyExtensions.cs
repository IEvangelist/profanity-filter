// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

public static class ReplacementStrategyExtensions
{
    public static string ToSummaryString(this ReplacementStrategy replacementStrategy)
    {
        return replacementStrategy switch
        {
            ReplacementStrategy.Emoji => "emoji",
            ReplacementStrategy.MiddleSwearEmoji => "middle swear emoji",
            ReplacementStrategy.RandomAsterisk => "random asterisk",
            ReplacementStrategy.MiddleAsterisk => "middle asterisk",
            ReplacementStrategy.VowelAsterisk => "vowel asterisk",
            ReplacementStrategy.FirstLetterThenAsterisk => "first letter then asterisk",
            ReplacementStrategy.AngerEmoji => "anger emoji",
            ReplacementStrategy.Bleep => "bleep",
            ReplacementStrategy.RedactedRectangle => "redacted rectangle",
            ReplacementStrategy.StrikeThrough => "string through",
            ReplacementStrategy.Underscores => "underscores",
            ReplacementStrategy.Grawlix => "grawlix",
            ReplacementStrategy.BoldGrawlix => "bold grawlix",

            _ => "asterisk",
        };
    }

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
