// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentCensorService : IProfaneContentCensorService
{
    // Been averaging around 50ms on my machine...
    private static TimeSpan? s_readAllWordsTimeSpan;

    private static readonly TimeSpan s_emitDebugIfLongerThan = TimeSpan.FromMilliseconds(100);
    private static readonly string[] s_resourceNames =
    [
        "ArabicSwearWords",
        "BritishSwearWords",
        "ChineseSwearWords",
        "FrenchSwearWords",
        "GermanSwearWords",
        "GoogleBannedWords",
        "IndonesianSwearWords",
        "ItalianSwearWords",
        "SpanishSwearWords",
    ];

    private static readonly AsyncLazy<HashSet<string>> s_profaneWords = new(factory: ReadAllProfaneWordsAsync);

    /// <summary>
    /// Reads all profane words from embedded resources asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that 
    /// returns a <see cref="HashSet{T}"/> of all profane words.</returns>
    private static async Task<HashSet<string>> ReadAllProfaneWordsAsync()
    {
        var startingTimestamp = Stopwatch.GetTimestamp();

        ConcurrentBag<string> words = [];

        await Parallel.ForEachAsync(s_resourceNames,
            async (resourceName, cancellationToken) =>
            {
                var qualifiedResourceName = $"""
                    ProfanityFilter.Services.Data.{resourceName}.txt
                    """;

                var content = await EmbeddedResourceReader.ReadAsync(
                    qualifiedResourceName, cancellationToken);

                if (string.IsNullOrWhiteSpace(content) is false)
                {
                    var wordList = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    for (var index = 0; index < wordList.Length; ++index)
                    {
                        var word = wordList[index];

                        var escapedWord = Regex.Escape(word);

                        words.Add(escapedWord);
                    }
                }
            });

        var set = words.ToHashSet();

        s_readAllWordsTimeSpan = Stopwatch.GetElapsedTime(startingTimestamp);

        return set;
    }

    /// <inheritdoc />
    async ValueTask<bool> IProfaneContentCensorService.ContainsProfanityAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var profaneWordList = await s_profaneWords.Task;

        Debug.Assert(
            s_readAllWordsTimeSpan.GetValueOrDefault() < s_emitDebugIfLongerThan,
            "Loading all of the embedded resources too longer than 100ms.");

        var pattern = $"\\b({string.Join('|', profaneWordList)})\\b";

        return Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase);
    }

    /// <inheritdoc />
    async ValueTask<string> IProfaneContentCensorService.CensorProfanityAsync(string content, ReplacementType replacementType)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        var profaneWordList = await s_profaneWords.Task;

        Debug.Assert(
            s_readAllWordsTimeSpan.GetValueOrDefault() < s_emitDebugIfLongerThan,
            "Loading all of the embedded resources too longer than 100ms.");

        var pattern = $"\\b({string.Join('|', profaneWordList)})\\b";

        var evaluator = replacementType switch
        {
            ReplacementType.Asterisk => MatchEvaluators.AsteriskEvaluator,
            _ => MatchEvaluators.EmojiEvaluator,
        };

        return Regex.Replace(content, pattern, evaluator, RegexOptions.IgnoreCase);
    }
}
