// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentCensorService(IMemoryCache cache) : IProfaneContentCensorService
{
    private const string ProfaneListKey = nameof(ProfaneListKey);

    /// <summary>
    /// Reads all profane words from their respective sources asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that 
    /// returns a readonly dictionary of all profane words.</returns>
    private async Task<Dictionary<string, ProfaneFilter>> ReadAllProfaneWordsAsync()
    {
        return await cache.GetOrCreateAsync(ProfaneListKey, async entry =>
        {
            var fileNames = ProfaneContentReader.GetFileNames();

            Console.WriteLine("Source word list for profane content:");

            foreach (var fileName in fileNames)
            {
                Console.WriteLine(fileName);
            }

            ConcurrentDictionary<string, List<string>> allWords = new(
                fileNames.Select(
                    static fileName => new KeyValuePair<string, List<string>>(fileName, [])));

            await Parallel.ForEachAsync(fileNames,
                async (fileName, cancellationToken) =>
                {
                    var content = await ProfaneContentReader.ReadAsync(
                        fileName, cancellationToken);

                    if (string.IsNullOrWhiteSpace(content) is false)
                    {
                        var words = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                        for (var index = 0; index < words.Length; ++index)
                        {
                            var word = words[index];

                            var escapedWord = Regex.Escape(word);

                            allWords[fileName].Add(escapedWord);
                        }
                    }
                })
                .ConfigureAwait(false);

            foreach (var (source, set) in allWords)
            {
                cache.Set(source, set.ToFrozenSet());
            }

            return allWords.ToDictionary(
                static kvp => kvp.Key,
                static kvp => new ProfaneFilter(kvp.Key, kvp.Value.ToFrozenSet()));
        }) ?? [];        
    }

    /// <inheritdoc />
    async ValueTask<FilterResult> IProfaneContentCensorService.CensorProfanityAsync(
        string content,
        ReplacementStrategy replacementStrategy)
    {
        FilterResult result = new(content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return result;
        }

        var evaluator = ToMatchEvaluator(replacementStrategy);

        var wordList =
            await ReadAllProfaneWordsAsync().ConfigureAwait(false);

        var stepContent = content;

        foreach (var (source, filter) in wordList)
        {
            if (result is { Steps: null } or { Steps.Count: 0 })
            {
                result = result with
                {
                    Steps = []
                };
            }

            FilterStep step = new(stepContent, source);

            var potentiallyReplacedContent =
                Regex.Replace(stepContent, filter.RegexPattern, evaluator, options: RegexOptions.IgnoreCase);

            if (stepContent != potentiallyReplacedContent)
            {
                stepContent = potentiallyReplacedContent;

                step = step with
                {
                    Output = potentiallyReplacedContent
                };
            }

            result.Steps.Add(step);
        }

        return result;
    }

    private static MatchEvaluator ToMatchEvaluator(ReplacementStrategy replacementStrategy)
    {
        return replacementStrategy switch
        {
            ReplacementStrategy.Asterisk => MatchEvaluators.AsteriskEvaluator,
            ReplacementStrategy.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementStrategy.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementStrategy.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementStrategy.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,
            ReplacementStrategy.AngerEmoji => MatchEvaluators.AngerEmojiEvaluator,

            _ => MatchEvaluators.EmojiEvaluator,
        };
    }
}
