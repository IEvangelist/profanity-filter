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
    private async Task<IEnumerable<KeyValuePair<string, FrozenSet<string>>>> ReadAllProfaneWordsAsync()
    {
        return await cache.GetOrCreateAsync(ProfaneListKey, static async entry =>
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

            return allWords.Select(
                static kvp =>
                    new KeyValuePair<string, FrozenSet<string>>(kvp.Key, kvp.Value.ToFrozenSet()));
        }) ?? [];        
    }

    /// <inheritdoc />
    async ValueTask<bool> IProfaneContentCensorService.ContainsProfanityAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var pattern = await GetProfaneWordListRegexPatternAsync();

        return pattern switch
        {
            null => false,
            _ => Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase)
        };
    }

    /// <inheritdoc />
    async ValueTask<CensorResult> IProfaneContentCensorService.CensorProfanityAsync(
        string content,
        ReplacementStrategy replacementStrategy)
    {
        CensorResult result = new(content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return result;
        }

        var evaluator = replacementStrategy switch
        {
            ReplacementStrategy.Asterisk => MatchEvaluators.AsteriskEvaluator,
            ReplacementStrategy.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementStrategy.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementStrategy.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementStrategy.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,
            ReplacementStrategy.AngerEmoji => MatchEvaluators.AngerEmojiEvaluator,

            _ => MatchEvaluators.EmojiEvaluator,
        };

        var wordList =
            await ReadAllProfaneWordsAsync().ConfigureAwait(false);

        var stepContent = content;

        foreach (var (source, set) in wordList)
        {
            var pattern = ToWordListRegexPattern(set);

            if (result is { Steps: null } or { Steps.Count: 0 })
            {
                result = result with
                {
                    Steps = []
                };
            }

            CensorStep step = new(stepContent, source);

            var potentiallyReplacedContent =
                Regex.Replace(stepContent, pattern, evaluator, options: RegexOptions.IgnoreCase);

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

    private async ValueTask<string?> GetProfaneWordListRegexPatternAsync()
    {
        var wordList =
            await ReadAllProfaneWordsAsync().ConfigureAwait(false);

        var set = wordList.SelectMany(
                static kvp => kvp.Value
            )
            .Distinct()
            .ToFrozenSet();

        return ToWordListRegexPattern(set);
    }

    private static string ToWordListRegexPattern(FrozenSet<string> set)
    {
        if (set.Count is 0)
        {
            Console.WriteLine("Unable to read profane word lists.");
            return "";
        }
        else
        {
            Console.WriteLine($"Found {set.Count:#,#} profane source words.");
        }

        var pattern = $"\\b({string.Join('|', set)})\\b";

        return pattern;
    }
}
