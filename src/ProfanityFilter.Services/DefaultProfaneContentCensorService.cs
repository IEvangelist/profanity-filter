// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentCensorService : IProfaneContentCensorService
{
    private static readonly AsyncLazy<HashSet<string>> s_getProfaneWords =
        new(factory: ReadAllProfaneWordsAsync);

    /// <summary>
    /// Reads all profane words from embedded resources asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that 
    /// returns a <see cref="HashSet{T}"/> of all profane words.</returns>
    private static async Task<HashSet<string>> ReadAllProfaneWordsAsync()
    {
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
            })
            .ConfigureAwait(false);

        var set = allWords.ToHashSet();

        return set;
    }

    /// <inheritdoc />
    async ValueTask<bool> IProfaneContentCensorService.ContainsProfanityAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var wordSet = await s_getProfaneWords.Task;

        if (wordSet.Count is 0)
        {
            Console.WriteLine("Unable to read profane word lists.");
            return false;
        }

        var pattern = $"\\b({string.Join('|', wordSet)})\\b";

        return Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase);
    }

    /// <inheritdoc />
    async ValueTask<string> IProfaneContentCensorService.CensorProfanityAsync(string content, ReplacementType replacementType)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        var wordSet = await s_getProfaneWords.Task;

        if (wordSet.Count is 0)
        {
            Console.WriteLine("Unable to read profane word lists.");
            return content;
        }

        var pattern = $"\\b({string.Join('|', wordSet)})\\b";

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
