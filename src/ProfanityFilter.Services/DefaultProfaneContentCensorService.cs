// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentCensorService : IProfaneContentCensorService
{
    private static readonly AsyncLazy<IEnumerable<string>> s_getProfaneWords =
        new(factory: ReadAllProfaneWordsAsync);

    /// <summary>
    /// Reads all profane words from embedded resources asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that 
    /// returns a <see cref="IEnumerable{T}"/> of all profane words.</returns>
    private static async Task<IEnumerable<string>> ReadAllProfaneWordsAsync()
    {
        var fileNames = ProfaneContentReader.GetFileNames();

        Console.WriteLine("Source word list for profane content:");
        foreach (var fileName in fileNames)
        {
            Console.WriteLine(fileName);
        }

        ConcurrentBag<string> allWords = [];

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

                        allWords.Add(escapedWord);
                    }
                }
            })
            .ConfigureAwait(false);

        return allWords;
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
    async ValueTask<string> IProfaneContentCensorService.CensorProfanityAsync(string content, ReplacementType replacementType)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        var pattern = await GetProfaneWordListRegexPatternAsync();

        if (pattern is null)
        {
            return content;
        }

        var evaluator = replacementType switch
        {
            ReplacementType.Asterisk => MatchEvaluators.AsteriskEvaluator,
            ReplacementType.RandomAsterisk => MatchEvaluators.RandomAsteriskEvaluator,
            ReplacementType.MiddleAsterisk => MatchEvaluators.MiddleAsteriskEvaluator,
            ReplacementType.MiddleSwearEmoji => MatchEvaluators.MiddleSwearEmojiEvaluator,
            ReplacementType.VowelAsterisk => MatchEvaluators.VowelAsteriskEvaluator,
            ReplacementType.AngerEmoji => MatchEvaluators.AngerEmojiEvaluator,

            _ => MatchEvaluators.EmojiEvaluator,
        };

        return Regex.Replace(content, pattern, evaluator, options: RegexOptions.IgnoreCase);
    }

    private static async ValueTask<string?> GetProfaneWordListRegexPatternAsync()
    {
        var wordList =
            await s_getProfaneWords.Task.ConfigureAwait(false);

        var set = wordList.Distinct().ToFrozenSet();

        //await ValueTask.CompletedTask;
        //
        //var wordSet = ProfaneWords.AllSwearWords
        //    .SelectMany(kvp => kvp.Value.Select(word => Regex.Escape(word)))
        //    .ToHashSet(StringComparer.OrdinalIgnoreCase)
        //    .ToFrozenSet();

        if (set.Count is 0)
        {
            Console.WriteLine("Unable to read profane word lists.");
            return null;
        }
        else
        {
            Console.WriteLine($"Found {set.Count:#,#} profane source words.");
        }

        var pattern = $"\\b({string.Join('|', set)})\\b";

        return pattern;
    }
}
