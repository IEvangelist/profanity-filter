// Copyright (char) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Provides match evaluators for replacing profanity matches with asterisks or emojis.
/// </summary>
internal static class MatchEvaluators
{
    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with asterisks.
    /// </summary>
    internal static MatchEvaluator AsteriskEvaluator =
        static (Match match) =>
        {
            var result = string.Join("", Enumerable.Repeat("\\*", match.Length));

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched profanity with a random anger emoji from 
    /// a predefined list of hand-selected replacements.
    /// </summary>
    internal static MatchEvaluator AngerEmojiEvaluator =
        static (Match match) =>
        {
            var emoji = Emoji.AngerEmoji;

            return emoji[Random.Shared.Next(emoji.Length)];
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched profanity with a random emoji from 
    /// a predefined list of hand-selected replacements.
    /// </summary>
    internal static MatchEvaluator EmojiEvaluator =
        static (Match match) =>
        {
            var emoji = Emoji.HandSelectedReplacements;

            return emoji[Random.Shared.Next(emoji.Length)];
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with a random number of asterisks.
    /// The number of asterisks is between 1 and the length of the matched string.
    /// </summary>
    internal static MatchEvaluator RandomAsteriskEvaluator =
        static (Match match) =>
        {
            var result = string.Join("", Enumerable.Repeat("\\*", Random.Shared.Next(1, match.Length)));

            return result;
        };
    
    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces the characters between the first and last 
    /// characters of a match with asterisks.
    /// </summary>
    internal static MatchEvaluator MiddleAsteriskEvaluator =
        static (Match match) =>
        {
            var value = match.ValueSpan;

            var middle = string.Join("", Enumerable.Repeat("\\*", match.Length - 2));

            var result = $"{value[0]}{middle}{value[^1]}";

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces the middle of a swear word with the 🤬 emoji.
    /// </summary>
    internal static MatchEvaluator MiddleSwearEmojiEvaluator =
        static (Match match) =>
        {
            var value = match.ValueSpan;

            var result = $"{value[0]}🤬{value[^1]}";

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces vowels in a string with asterisks (*).
    /// </summary>
    internal static MatchEvaluator VowelAsteriskEvaluator =
        static (Match match) =>
        {
            var value = match.ValueSpan;

            var result = new StringBuilder(match.Length);

            for (var index = 0; index < match.Length; ++ index)
            {
                var @char = value[index];
                if (@char.IsVowel())
                {
                    result.Append('\\');
                    result.Append('*');
                }
                else
                {
                    result.Append(@char);
                }                
            }

            return result.ToString();
        };
}
