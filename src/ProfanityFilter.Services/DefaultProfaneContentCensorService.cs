// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentCensorService : IProfaneContentCensorService
{
    // Been averaging around 50ms on my machine...
    private static TimeSpan? s_readAllWordsTimeSpan;

    private static readonly TimeSpan s_emitDebugIfLongerThan =
        TimeSpan.FromMilliseconds(250);
    private static readonly AsyncLazy<HashSet<string>> s_profaneWords =
        new(factory: ReadAllProfaneWordsAsync);

    /// <summary>
    /// Reads all profane words from embedded resources asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that 
    /// returns a <see cref="HashSet{T}"/> of all profane words.</returns>
    private static async Task<HashSet<string>> ReadAllProfaneWordsAsync()
    {
        var startingTimestamp = Stopwatch.GetTimestamp();

        var fileNames = ProfaneContentReader.GetFileNames();

        ConcurrentBag<string> allWords = [];

        await Parallel.ForEachAsync(fileNames,
            async (fileName, cancellationToken) =>
            {
                var content = await ProfaneContentReader.ReadAsync(
                    fileName, cancellationToken);

                if (content.Words?.Length > 0)
                {
                    var words = content.Words;
                    for (var index = 0; index < words.Length; ++index)
                    {
                        var word = words[index];

                        var escapedWord = Regex.Escape(word);

                        allWords.Add(escapedWord);
                    }
                }
            });

        var set = allWords.ToHashSet();

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
            $"""
                Loading all of the word JSON content too longer than {s_emitDebugIfLongerThan.Milliseconds}ms.
                The total time was {s_readAllWordsTimeSpan.GetValueOrDefault().TotalMilliseconds}ms.
                """);

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
            $"""
                Loading all of the word JSON content too longer than {s_emitDebugIfLongerThan.Milliseconds}ms.
                The total time was {s_readAllWordsTimeSpan.GetValueOrDefault().TotalMilliseconds}ms.
                """);

        var pattern = $"\\b({string.Join('|', profaneWordList)})\\b";

        var evaluator = replacementType switch
        {
            ReplacementType.Asterisk => MatchEvaluators.AsteriskEvaluator,
            ReplacementType.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementType.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementType.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementType.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,

            _ => MatchEvaluators.EmojiEvaluator,
        };

        return Regex.Replace(content, pattern, evaluator, options: RegexOptions.IgnoreCase);
    }
}
