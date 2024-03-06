// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class CoreServiceExtensions
{
    private const string UpdatedNote =
        "This content was automatically updated to _filter out profanity_. " +
        "If you feel as though this was done in error, please " +
        "[file an issue with the details](https://github.com/IEvangelist/profanity-filter/issues/new).";

    /// <summary>
    /// Gets the final result text based on the given <paramref name="result"/>'s final output
    /// and the <paramref name="core"/> service's input for <c>include-updated-note</c>.
    /// </summary>
    public static string? GetFinalResultText(this ICoreService core, FilterResult? result)
    {
        if (result is null)
        {
            return null;
        }

        if (result.IsFiltered == false)
        {
            return result.Input;
        }

        var finalResult = result.FinalOutput;

        // We do not add updated notes to titles.
        if (result.Parameters.Target is FilterTarget.Title)
        {
            return finalResult;
        }

        // Consumers can opt-out of this feature.
        if (core.GetBoolInput(ActionInputs.IncludeUpdatedNote) is false)
        {
            return finalResult;
        }

        // If the result already contains the note, we don't need to add it again.
        // This can occur if a user has already filed an issue, and we've updated the content.
        if (finalResult.Contains(UpdatedNote))
        {
            return finalResult;
        }

        return $"""
            {finalResult}

            > [!NOTE]
            > {UpdatedNote}
            """;
    }

    /// <summary>
    /// Gets a value indicating whether to react with a <c>confused</c> emoji when a profane word is found.
    /// </summary>
    public static bool IncludeConfusedReaction(this ICoreService core) =>
        core.GetBoolInput(ActionInputs.IncludeConfusedReaction);

    /// <summary>
    /// The <c>replacement-strategy</c> to use when filtering profane content.
    /// Example valid values: <c>anger-asterisk</c>, <c>AngerAsterisk</c>
    /// </summary>
    public static ReplacementStrategy GetReplacementStrategy(this ICoreService core)
    {
        return core.GetInput(ActionInputs.ReplacementStrategy) is string value
            && Enum.TryParse<ReplacementStrategy>(
                value: NormalizeEnumString(value),
                ignoreCase: true,
                result: out var strategy)
                    ? strategy
                    : ReplacementStrategy.Asterisk;

        // The values are case-insensitive, and we normalize them to remove "-".
        static string NormalizeEnumString(string enumValue)
        {
            return enumValue.Replace("-", "");
        }
    }

    /// <summary>
    /// Gets the manual profane words from the action's input <c>manual-profane-words</c> value.
    /// </summary>
    public static string[]? GetManualProfaneWords(this ICoreService core)
    {
        var words = core.GetInput(ActionInputs.ManualProfaneWords);

        if (words is null or { Length: 0 })
        {
            return default;
        }

        return words.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(static word => word.Trim())
            .ToArray();
    }

    private static readonly HttpClient s_client = new();
    private static readonly string[] s_newLines = ["\r\n", "\r", "\n"];

    /// <summary>
    /// Gets the custom profane words from the action's input <c>custom-profane-words-url</c> value,
    /// making an HTTP GET call to the specified URL.
    /// </summary>
    public static async ValueTask<string[]?> GetCustomProfaneWordsAsync(this ICoreService core)
    {
        if (core.GetInput(ActionInputs.CustomProfaneWordsUrl) is { Length: > 0 } requestUri)
        {
            try
            {
                var words = await s_client.GetStringAsync(requestUri);

                if (words is null or { Length: 0 })
                {
                    return default;
                }

                return words.Split(s_newLines, StringSplitOptions.RemoveEmptyEntries)
                    .Select(static word => word.Trim())
                    .ToArray();
            }
            catch (Exception ex)
            {
                core.WriteWarning($"""
                    Unable to get the custom word list at {requestUri}:
                    {ex}
                    """);
            }
        }

        return default;
    }
}
