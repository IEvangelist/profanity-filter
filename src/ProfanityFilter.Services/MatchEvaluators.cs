// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Provides match evaluators for replacing profanity matches with asterisks or emojis.
/// </summary>
internal static class MatchEvaluators
{
    /// <summary>
    /// A match evaluator that replaces the matched string with asterisks (*).
    /// </summary>
    internal static MatchEvaluator AsteriskEvaluator = new(
        static match =>
        {
            var result = new string('*', match.Length);

            return result;
        });

    /// <summary>
    /// A match evaluator that replaces profanity matches with a random hand-selected emoji.
    /// </summary>
    internal static MatchEvaluator EmojiEvaluator = new(
        static match =>
        {
            var emoji = Emoji.HandSelectedReplacements;

            return emoji[Random.Shared.Next(emoji.Length)];
        });

    // TODO: Consider additional replacement types/match evaluator pairs...
}
