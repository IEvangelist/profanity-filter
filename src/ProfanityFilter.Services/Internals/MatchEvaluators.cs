// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Provides match evaluators for replacing profanity matches with asterisks or emojis.
/// </summary>
internal static class MatchEvaluators
{
    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with grawlix symbols.
    /// </summary>
    internal static MatchEvaluator GrawlixEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            var grawlixes = isNotTitle ? Symbols.Grawlixes : Symbols.UnescapedGrawlixes;

            var limit = isNotTitle ? Symbols.LimitGrawlixToOne : Symbols.UnescapedLimitGrawlixToOne;

            var randomGrawlix = grawlixes.RandomItemsWithLimitToOne(match.Length, limit);

            var result = string.Join(
                "", randomGrawlix);

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with grawlix symbols.
    /// </summary>
    internal static MatchEvaluator BoldGrawlixEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            var grawlixes = isNotTitle ? Symbols.Grawlixes : Symbols.UnescapedGrawlixes;

            var limit = isNotTitle ? Symbols.LimitGrawlixToOne : Symbols.UnescapedLimitGrawlixToOne;

            var randomGrawlix = grawlixes.RandomItemsWithLimitToOne(match.Length, limit);

            var result = string.Join(
                "", randomGrawlix);

            // Titles cannot be bolded
            return isNotTitle ? $"__{result}__" : result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with asterisks.
    /// </summary>
    internal static MatchEvaluator AsteriskEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            // Don't escape titles
            var element = isNotTitle ? "\\*" : "*";

            var result = string.Join(
                "", Enumerable.Repeat(element, match.Length));

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched profanity with a random emoji from 
    /// a predefined list of hand-selected replacements.
    /// </summary>
    internal static MatchEvaluator EmojiEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var emoji = Emoji.HandSelectedReplacements;

            return emoji[Random.Shared.Next(emoji.Length)];
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched profanity with a random anger emoji from 
    /// a predefined list of hand-selected replacements.
    /// </summary>
    internal static MatchEvaluator AngerEmojiEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var emoji = Emoji.AngerEmoji;

            return emoji[Random.Shared.Next(emoji.Length)];
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces the everythingAfterFirstLetter of a swear word with the 🤬 emoji.
    /// </summary>
    internal static MatchEvaluator MiddleSwearEmojiEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var value = match.ValueSpan;

            var result = $"{value[0]}🤬{value[^1]}";

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with a random number of asterisks.
    /// The number of asterisks is between 1 and the length of the matched string.
    /// </summary>
    internal static MatchEvaluator RandomAsteriskEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            // Don't escape titles
            var element = isNotTitle ? "\\*" : "*";

            var result = string.Join(
                "", Enumerable.Repeat(element, Random.Shared.Next(1, match.Length)));

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces the characters between the first and last 
    /// characters of a match with asterisks.
    /// </summary>
    internal static MatchEvaluator MiddleAsteriskEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var value = match.ValueSpan;

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            // Don't escape titles
            var element = isNotTitle ? "\\*" : "*";

            var middle = string.Join(
                "", Enumerable.Repeat(element, match.Length - 2));

            var result = $"{value[0]}{middle}{value[^1]}";

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces everything after the first letter in a string with asterisks (*).
    /// </summary>
    internal static MatchEvaluator FirstLetterThenAsteriskEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var value = match.ValueSpan;

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            // Don't escape titles
            var element = isNotTitle ? "\\*" : "*";

            var everythingAfterFirstLetter = string.Join(
                "", Enumerable.Repeat(element, match.Length - 1));

            var result = $"{value[0]}{everythingAfterFirstLetter}";

            return result;
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces vowels in a string with asterisks (*).
    /// </summary>
    internal static MatchEvaluator VowelAsteriskEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var value = match.ValueSpan;

            var result = new StringBuilder(match.Length);

            var isNotTitle = parameters.Target is not FilterTarget.Title;

            for (var index = 0; index < match.Length; ++index)
            {
                var @char = value[index];
                if (@char.IsVowel())
                {
                    if (isNotTitle) // Don't escape titles
                    {
                        result.Append('\\');
                    }

                    result.Append('*');
                }
                else
                {
                    result.Append(@char);
                }
            }

            return result.ToString();
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with the word "bleep".
    /// </summary>
    internal static MatchEvaluator BleepEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            return "bleep";
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces each letter in a string with the rectangle symbol <c>█</c>.
    /// </summary>
    internal static MatchEvaluator RedactedRectangleEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var length = match.Length;

            return new string('█', length);
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with the <c>~~struck through~~</c> markdown.
    /// </summary>
    internal static MatchEvaluator StrikeThroughEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            if (parameters.Target is FilterTarget.Title)
            {
                return match.Value; // We cannot strike through a title.
            }

            var value = match.ValueSpan;

            return $"~~{value}~~";
        };

    /// <summary>
    /// A <see cref="MatchEvaluator"/> that replaces a matched string with underscored.
    /// </summary>
    internal static MatchEvaluator UnderscoresEvaluator(FilterParameters parameters) =>
        string (match) =>
        {
            MatchRegistry.Register(parameters, match);

            var length = match.Length;

            return new string('_', length);
        };
}
