// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ReplacementStrategyExtensions
{
    /// <summary>
    /// Gets a summary-friendly string representation of the given <paramref name="replacementStrategy"/>.
    /// </summary>
    internal static string ToSummaryString(this ReplacementStrategy replacementStrategy)
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
}
