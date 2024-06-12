// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class DefaultProfaneContentFilterService(IMemoryCache cache) : IProfaneContentFilterService
{
    private const string ProfaneListKey = nameof(ProfaneListKey);

    /// <inheritdoc />
    public async Task<Dictionary<string, ProfaneSourceFilter>> ReadAllProfaneWordsAsync()
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
                static kvp => new ProfaneSourceFilter(kvp.Key, kvp.Value.ToFrozenSet()));
        }) ?? [];        
    }

    /// <inheritdoc />
    public async ValueTask<FilterResult> FilterProfanityAsync(
        string content,
        FilterParameters parameters)
    {
        var (strategy, target) = parameters;
        FilterResult result = new(content, parameters);

        if (string.IsNullOrWhiteSpace(content))
        {
            return result;
        }

        var wordList =
            await ReadAllProfaneWordsAsync().ConfigureAwait(false);

        foreach (var profaneSourceFilter in parameters.AdditionalFilterSources ?? [])
        {
            wordList[profaneSourceFilter.SourceName] = profaneSourceFilter;
        }

        var getEvaluator = strategy.GetMatchEvaluator();
        var evaluator = getEvaluator(parameters);

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

        return result with
        {
            Matches = MatchRegistry.Unregister(parameters)
        };
    }
}
